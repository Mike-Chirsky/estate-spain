(function(a){if(typeof define==="function"&&define.amd){define(["jquery"],a)}else{a(window.jQuery||window.Zepto)}}(function(d){var f=[],b={},e=/^([a-z]:)?\/\//i,c=/\.\w+$/,a;b.init=function(g){g=d.extend({ping:null,dimensions:"preserve",glue:"_",skipExtensions:["svg"]},g);this.each(function(){var k=d(this);if(!k.is("img")||k.hasClass("dense-image")){return}k.addClass("dense-image dense-loading");var j=b.getImageAttribute.call(this),i=k.attr("src"),l=false,h;if(!j){if(!i||a===1||d.inArray(i.split(".").pop().split(/[\?\#]/).shift(),g.skipExtensions)!==-1){k.removeClass("dense-image dense-loading");return}j=i.replace(c,function(m){return g.glue+a+"x"+m});l=g.ping!==false&&d.inArray(j,f)===-1&&(g.ping===true||!e.test(j)||j.indexOf("//"+document.domain)===0||j.indexOf(document.location.protocol+"//"+document.domain)===0)}h=function(){var m=function(){k.removeClass("dense-loading").addClass("dense-ready").trigger("denseRetinaReady.dense")};k.attr("src",j);if(g.dimensions==="update"){k.dense("updateDimensions").one("denseDimensionChanged",m)}else{if(g.dimensions==="remove"){k.removeAttr("width height")}m()}};if(l){d.ajax({url:j,type:"HEAD"}).done(function(o,p,n){var m=n.getResponseHeader("Content-type");if(!m||m.indexOf("image/")===0){f.push(j);h()}})}else{h()}});return this};b.updateDimensions=function(){return this.each(function(){var g,h=d(this),i=h.attr("src");if(i){g=new Image();g.src=i;d(g).on("load.dense",function(){h.attr({width:g.width,height:g.height}).trigger("denseDimensionChanged.dense")})}})};b.devicePixelRatio=function(){var g=1;if(d.type(window.devicePixelRatio)!=="undefined"){g=window.devicePixelRatio}else{if(d.type(window.matchMedia)!=="undefined"){d.each([1.3,2,3,4,5,6],function(h,i){var j=["(-webkit-min-device-pixel-ratio: "+i+")","(min-resolution: "+Math.floor(i*96)+"dpi)","(min-resolution: "+i+"dppx)"].join(",");if(!window.matchMedia(j).matches){return false}g=i})}}return Math.ceil(g)};b.getImageAttribute=function(){var k=d(this).eq(0),j=false,g;for(var h=1;h<=a;h++){g=k.attr("data-"+h+"x");if(g){j=g}}return j};a=b.devicePixelRatio();d.fn.dense=function(h,g){if(d.type(h)!=="string"||d.type(b[h])!=="function"){g=h;h="init"}return b[h].call(this,g)};d(function(){d("body.dense-retina img").dense()})}));