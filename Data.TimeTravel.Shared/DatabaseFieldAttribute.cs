namespace Data.TimeTravel.Shared
{
    public class DatabaseFieldAttribute : Attribute
    {
        public string AttributeName { get; set; }
        public string ListName { get; set; }
        public Type AttributeType { get; set; }

        public DatabaseFieldAttribute(string attributeName, string listName, Type attributeType)
        {
            this.AttributeName = attributeName;
            this.ListName = listName;
            this.AttributeType = attributeType;
        }
    }
}