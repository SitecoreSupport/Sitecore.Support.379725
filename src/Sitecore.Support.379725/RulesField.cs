using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Rules;

namespace Sitecore.Support.Data.Fields.RulesField
{
    public class RulesField : Sitecore.Data.Fields.RulesField
    {
        private XDocument rulesDefinitionDocument;
        private XDocument RulesDefinitionDocument
        {
            get
            {
                if (rulesDefinitionDocument == null && !string.IsNullOrEmpty(base.Value))
                {
                    try
                    {
                        rulesDefinitionDocument = XDocument.Parse(base.Value);
                    }
                    catch (Exception owner)
                    {
                        Log.Error("Invalid value in the Rules field type. Item: " + AuditFormatter.FormatItem(base.InnerField.Item) + ". Field: " + AuditFormatter.FormatField(base.InnerField) + ".", owner);
                    }
                }
                return rulesDefinitionDocument;
            }
        }
        public RulesField(Field innerField)
            : base(innerField)
        {
            Assert.ArgumentNotNull(innerField, "innerField");
        }
        public RulesField(Field innerField, string runtimeValue)
            : base(innerField, runtimeValue)
        {
            Assert.ArgumentNotNull(innerField, "innerField");
            Assert.ArgumentNotNull(runtimeValue, "runtimeValue");
        }
        public override void ValidateLinks(LinksValidationResult result)
        {
            Assert.ArgumentNotNull(result, "result");
            if (RulesDefinitionDocument != null)
            {
                Database database = base.InnerField.Database;
                RulesDefinition rulesDefinition = new RulesDefinition(RulesDefinitionDocument.ToString());
                List<ID> list = new List<ID>();
                list.AddRange(rulesDefinition.GetReferencedActions());
                list.AddRange(rulesDefinition.GetReferencedConditions());
                list.AddRange(rulesDefinition.GetReferencedItems());
                foreach (ID item2 in list)
                {
                    Item item = database.GetItem(item2);
                    if (item != null)
                    {
                        result.AddValidLink(item, item.Paths.Path);
                    }
                    else
                    {
                        result.AddBrokenLink(item2.ToString());
                    }
                }
            }
        }
    }
}