﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    [XmlRoot("response")]
    public class CaseResponseRoot
    {
        [XmlElement("cases")]
        public CaseCollection CaseList;
    }

    [XmlRoot("cases")]
    public class CaseCollection
    {
        [XmlElement("case")]
        public Case[] Cases;
    }
}
