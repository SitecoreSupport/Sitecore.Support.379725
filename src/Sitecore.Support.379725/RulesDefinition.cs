using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Sitecore.Support.Rules.RulesDefinition
{
    public class RulesDefinition : Sitecore.Rules.RulesDefinition
    {
        public RulesDefinition(string rulesXml): base(rulesXml)
        {
        }
        public List<ID> GetReferencedItems()
        {
            var referencedItems = new List<ID>();
            var attributes = this.Document.Descendants(ActionTagName).Attributes();
            attributes = attributes.Union(this.Document.Descendants(ConditionTagName).Attributes());
            foreach (var attribute in attributes)
            {
                if (string.Equals(IdAttributeName, attribute.Name.LocalName, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(UidAttributeName, attribute.Name.LocalName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                ID parsedId;
                List<string> innerIds = new List<string>();
                if (attribute.Value.Contains("|"))
                {
                    var ids = attribute.Value.Split('|');

                    foreach (var id in ids)
                    {
                        innerIds.Add(id);
                    }
                }
                else
                {
                    innerIds.Add(attribute.Value);
                }
                foreach (var id in innerIds)
                {
                    if (!ID.TryParse(id, out parsedId))
                    {
                        continue;
                    }

                    if (!referencedItems.Contains(parsedId))
                    {
                        referencedItems.Add(parsedId);
                    }
                }

            }

            return referencedItems;
        }
    }
}