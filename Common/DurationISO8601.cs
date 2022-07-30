namespace Common
{
    public static class Duration
    {
        public static DateTime? Parse(string durationStr)
        {
            if (String.IsNullOrWhiteSpace(durationStr))
            {
                return null;
            }

            if (!durationStr.StartsWith("P"))
            {
                throw new ArgumentException("String deveria começar com P");
            }

            var parsed = System.Xml.XmlConvert.ToTimeSpan(durationStr);
            return  DateTime.Parse(parsed.ToString());

        }
    }
}