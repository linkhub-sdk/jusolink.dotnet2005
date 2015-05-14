using System;
using System.Collections.Generic;

namespace Jusolink
{
    public class SearchResult
    {
        public String searches;
        public String suggest;
        public SidoCount sidoCount;
        public int? numFound;
        public int? listSize;
        public int? totalPage;
        public int? page;
        public bool? chargeYN;
        public List<JusoInfo> juso;
        public List<String> deletedWord;
    }
}