using System.Collections.Generic;

namespace HobknobClientNet
{
    public class Node
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public List<Node> Nodes { get; set; }
        public bool Dir { get; set; }
    }
}