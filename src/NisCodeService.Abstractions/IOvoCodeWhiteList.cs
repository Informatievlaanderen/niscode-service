namespace NisCodeService.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IOvoCodeWhiteList
    {
        bool IsWhiteListed(string ovoCode);
    }

    public class OvoCodeWhiteList : IOvoCodeWhiteList
    {
        private readonly IList<string> _ovoCodeWhiteList;

        public OvoCodeWhiteList(IList<string> ovoCodeWhiteList)
        {
            _ovoCodeWhiteList = ovoCodeWhiteList;
        }

        public bool IsWhiteListed(string ovoCode)
        {
            return _ovoCodeWhiteList.Contains(ovoCode, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
