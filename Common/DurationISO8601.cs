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
            return  new DateTime(parsed.Ticks);

        }
        
        public static string FromDateTime(DateTime dateTime)
        {
            var timeSpan = new TimeSpan(dateTime.Ticks);
            var durationFormatISO8601 = System.Xml.XmlConvert.ToString(timeSpan);
            return durationFormatISO8601;

        }
    }
}