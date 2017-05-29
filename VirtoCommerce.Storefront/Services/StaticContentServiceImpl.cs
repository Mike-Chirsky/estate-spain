﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.LiquidThemeEngine.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.StaticContent.Services;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Tools;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.Storefront.Services
{
    /// <summary>
    /// Represent a search and rendering static content (pages and blogs)
    /// </summary>
    public class StaticContentServiceImpl : IStaticContentService
    {
        private static readonly Regex _headerRegExp = new Regex(@"(?s:^---(.*?)---)");
        private static readonly string[] _extensions = { ".md", ".html" };
        private readonly ILiquidThemeEngine _liquidEngine;
        private readonly Func<WorkContext> _workContextFactory;
        private readonly Func<IStorefrontUrlBuilder> _urlBuilderFactory;
        private readonly Func<string, ContentItem> _contentItemFactory;
        private readonly IStaticContentBlobProvider _contentBlobProvider;
        private readonly MarkdownPipeline _markdownPipeline;

        [CLSCompliant(false)]
        public StaticContentServiceImpl(ILiquidThemeEngine liquidEngine,
                                        ILocalCacheManager cacheManager, Func<WorkContext> workContextFactory,
                                        Func<IStorefrontUrlBuilder> urlBuilderFactory, Func<string, ContentItem> contentItemFactory,
                                        IStaticContentBlobProvider contentBlobProvider)
        {
            _liquidEngine = liquidEngine;

            _workContextFactory = workContextFactory;
            _urlBuilderFactory = urlBuilderFactory;
            _contentItemFactory = contentItemFactory;
            _contentBlobProvider = contentBlobProvider;

            //Observe content changes to invalidate cache if changes occur
            _contentBlobProvider.Changed += (sender, args) =>
            {
                cacheManager.Clear();
            };
            _contentBlobProvider.Renamed += (sender, args) =>
            {
                cacheManager.Clear();
            };

            _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }

        #region IStaticContentService Members

        public IEnumerable<ContentItem> LoadStoreStaticContent(Store store)
        {
            var retVal = new List<ContentItem>();
            var baseStoreContentPath = "/" + store.Id;
            const string searchPattern = "*.*";

            if (_contentBlobProvider.PathExists(baseStoreContentPath))
            {
                var config = _liquidEngine.GetSettings();

                //Search files by requested search pattern
                var contentBlobs = _contentBlobProvider.Search(baseStoreContentPath, searchPattern, true)
                                             .Where(x => _extensions.Any(x.EndsWith))
                                             .Select(x => x.Replace("\\\\", "\\"));

                //each content file  has a name pattern {name}.{language?}.{ext}
                var localizedBlobs = contentBlobs.Select(x => new LocalizedBlobInfo(x));

                foreach (var localizedBlob in localizedBlobs.OrderBy(x => x.Name))
                {
                    var blobRelativePath = "/" + localizedBlob.Path.TrimStart('/');
                    var contentItem = _contentItemFactory(blobRelativePath);
                    if (contentItem != null)
                    {
                        if (contentItem.Name == null)
                        {
                            contentItem.Name = localizedBlob.Name;
                        }
                        contentItem.Language = localizedBlob.Language;
                        contentItem.FileName = Path.GetFileName(blobRelativePath);
                        contentItem.StoragePath = "/" + blobRelativePath.Replace(baseStoreContentPath + "/", string.Empty).TrimStart('/');

                        LoadAndRenderContentItem(blobRelativePath, contentItem);

                        retVal.Add(contentItem);
                    }
                }
            }

            return retVal.ToArray();
        }

        #endregion

        private void LoadAndRenderContentItem(string contentPath, ContentItem contentItem)
        {
            string content;
            using (var stream = _contentBlobProvider.OpenRead(contentPath))
            {
                //Load raw content with metadata
                content = stream.ReadToString();
            }

            IDictionary<string, IEnumerable<string>> metaHeaders;
            string error = null;

            try
            {
                metaHeaders = ReadYamlHeader(content);
            }
            catch (Exception ex)
            {
                error = $"Failed to parse YAML header from \"{contentItem.StoragePath}\"<br/>{ex.Message}";
                metaHeaders = new Dictionary<string, IEnumerable<string>>();
            }

            content = RemoveYamlHeader(content);

            IDictionary themeSettings = null;
            var workContext = _workContextFactory();
            if (workContext != null)
            {
                var shopifyContext = workContext.ToShopifyModel(_urlBuilderFactory());
                var parameters = (Dictionary<string, object>)shopifyContext.ToLiquid();

                themeSettings = _liquidEngine.GetSettings();
                parameters.Add("settings", themeSettings);
                //Render content by liquid engine
                content = _liquidEngine.RenderTemplate(content, parameters);
            }

            //Render markdown content
            if (Path.GetExtension(contentItem.StoragePath).EqualsInvariant(".md"))
            {
                content = Markdown.ToHtml(content, _markdownPipeline);
            }

            if (!string.IsNullOrEmpty(error))
            {
                content = $"{error}<br/>{content}";
            }

            contentItem.LoadContent(content, metaHeaders);

            //Try to get default permalink template from theme settings
            if (string.IsNullOrEmpty(contentItem.Permalink))
            {
                contentItem.Permalink = ":folder/:categories/:title";
                if (themeSettings != null)
                {
                    contentItem.Permalink = (string)themeSettings["permalink"] ?? contentItem.Permalink;
                }
            }
            //Transform permalink template to url
            contentItem.Url = GetContentItemUrl(contentItem, contentItem.Permalink);
            //Transform aliases permalink templates to urls
            contentItem.AliasesUrls = contentItem.Aliases.Select(x => GetContentItemUrl(contentItem, x)).ToList();
        }

        private static string GetContentItemUrl(ContentItem item, string permalink)
        {
            return new FrontMatterPermalink
            {
                UrlTemplate = permalink,
                Categories = item.Categories,
                Date = item.CreatedDate,
                FilePath = item.StoragePath
            }.ToUrl();
        }

        private static string RemoveYamlHeader(string text)
        {
            var retVal = text;
            var headerMatches = _headerRegExp.Matches(text);
            if (headerMatches.Count > 0)
            {
                retVal = text.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }
            return retVal;
        }

        private static IDictionary<string, IEnumerable<string>> ReadYamlHeader(string text)
        {
            var retVal = new Dictionary<string, IEnumerable<string>>();
            var headerMatches = _headerRegExp.Matches(text);
            if (headerMatches.Count == 0)
                return retVal;

            var input = new StringReader(headerMatches[0].Groups[1].Value);
            var yaml = new YamlStream();

            yaml.Load(input);

            if (yaml.Documents.Count > 0)
            {
                var root = yaml.Documents[0].RootNode;
                var collection = root as YamlMappingNode;
                if (collection != null)
                {
                    foreach (var entry in collection.Children)
                    {
                        var node = entry.Key as YamlScalarNode;
                        if (node != null)
                        {
                            retVal.Add(node.Value, GetYamlNodeValues(entry.Value));
                        }
                    }
                }
            }
            return retVal;
        }

        private static IEnumerable<string> GetYamlNodeValues(YamlNode value)
        {
            var retVal = new List<string>();
            var list = value as YamlSequenceNode;

            if (list != null)
            {
                retVal.AddRange(list.Children.OfType<YamlScalarNode>().Select(node => node.Value));
            }
            else
            {
                retVal.Add(value.ToString());
            }

            return retVal;
        }


        //each content file  has a name pattern {name}.{language?}.{ext}
        private class LocalizedBlobInfo
        {
            public LocalizedBlobInfo(string blobPath)
            {
                Path = blobPath;
                Language = Language.InvariantLanguage;

                var parts = System.IO.Path.GetFileName(blobPath)?.Split('.');
                Name = parts?.FirstOrDefault();

                if (parts?.Length == 3)
                {
                    try
                    {
                        Language = new Language(parts[1]);
                    }
                    catch (Exception)
                    {
                        Language = Language.InvariantLanguage;
                    }
                }
            }

            public string Name { get; }
            public Language Language { get; }
            public string Path { get; }
        }
    }
}
