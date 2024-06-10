using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMVLogger
{
    public class Aid
    {
        public string Name { get; set; }
        public List<Item> Item { get; set; }
    }

    public class Item
    {
        public string Tag { get; set; }
        public string Value { get; set; }
    }

    public class PublicKey
    {
        public string Rid { get; set; }
        public string Index { get; set; }
        public string Hash { get; set; }
        public string Exponent { get; set; }
        public string Modulus { get; set; }
        public string HashAlgorithm { get; set; }
        public string SignAlgorithm { get; set; }
    }

    public class ConfigRoot
    {
        public string CardReaderName { get; set; }
        public string ConfigEditApp { get; set; }
        public List<Aid> Contact { get; set; }
        public List<PublicKey> PublicKeys { get; set; }
    }

}
