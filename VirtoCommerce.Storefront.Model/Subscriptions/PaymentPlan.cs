﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    public class PaymentPlan
    {
        public PaymentPlan()
        {
            Interval = PaymentInterval.Months;
        }
        public string Id { get; set; }
        /// <summary>
        /// (days, months, years) - billing interval
        /// </summary>
        public PaymentInterval Interval { get; set; }
        /// <summary>
        /// - to set more customized intervals (every 5 month)
        /// </summary>
        public int IntervalCount { get; set; }
        /// <summary>
        ///  subscription trial period in days 
        /// </summary>
        public int TrialPeriodDays { get; set; }
    }
}
