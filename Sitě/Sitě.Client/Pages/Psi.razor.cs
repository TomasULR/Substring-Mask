namespace Sitě.Client.Pages
{
    public partial class Psi
    {
        private int SubnetCount { get; set; }
        private int DevicesPerSubnet { get; set; }
        private List<CalculationResult> resultsList = new List<CalculationResult>();


        private List<int?> inputValues = new List<int?>();
        private int lastAssignedIp = -1; // This will keep track of the last assigned IP address as an integer offset.

        void AddInput()
        {
            inputValues.Add(null);
        }

        void RemoveInput(int index)
        {
            inputValues.RemoveAt(index);
        }



        private void Calculate()
        {
            lastAssignedIp = -1;

            resultsList.Clear();
            var indexedInputs = inputValues.Select((hosts, index) => (hosts, index)).ToList();
            var sortedInputs = indexedInputs.OrderByDescending(pair => pair.hosts).ToList();
            var tempResults = new List<(CalculationResult result, int originalIndex)>();

            foreach (var item in sortedInputs)
            {
                int requiredHosts = item.hosts.HasValue ? item.hosts.Value + 2 : 0;

                int hostBits = (int)Math.Ceiling(Math.Log(requiredHosts, 2));

                int networkPrefix = 32 - hostBits;
                SubnetData? matchingSubnet = subnetData.FirstOrDefault(subnet => subnet.PrefixLength == networkPrefix);

                string subnetMask = matchingSubnet != null ? matchingSubnet.SubnetMask : "N/A";
                int maxHosts = (int)Math.Pow(2, 32 - networkPrefix) - 1;

                // Calculate the next network address based on the last assigned IP.
                int nextNetworkIp = lastAssignedIp + 1; // Increment to get to the next network.
                lastAssignedIp += maxHosts + 1; // Update lastAssignedIp to the last usable IP in this subnet.

                // Construct the network address string.
                string networkAddress = $"192.0.2.{nextNetworkIp}/{networkPrefix}";
                string firstHost = $"192.0.2.{nextNetworkIp + 1}/{networkPrefix}";
                string lastHost = $"192.0.2.{lastAssignedIp - 1}/{networkPrefix}";
                string broadcastAddress = $"192.0.2.{lastAssignedIp}/{networkPrefix}";

                var subnetResult = new CalculationResult
                {
                    NetworkRange = networkAddress,
                    BroadcastAddress = broadcastAddress,
                    FirstHost = firstHost,
                    LastHost = lastHost,
                    SubnetMask = subnetMask
                };
                if (networkPrefix > 8)
                {
                    tempResults.Add((subnetResult, item.index));
                }
            }

            //foreach (var (hosts, originalIndex) in sortedInputs)
            //{
            //    int requiredHosts = hosts.HasValue ? hosts.Value + 2 : 0;

            //    int hostBits = (int)Math.Ceiling(Math.Log(requiredHosts, 2));

            //    int networkPrefix = 32 - hostBits;
            //    SubnetData matchingSubnet = subnetData.FirstOrDefault(subnet => subnet.PrefixLength == networkPrefix);

            //    string subnetMask = matchingSubnet != null ? matchingSubnet.SubnetMask : "N/A";
            //    int maxHosts = (int)Math.Pow(2, 32 - networkPrefix) - 1;

            //    // Calculate the next network address based on the last assigned IP.
            //    int nextNetworkIp = lastAssignedIp + 1; // Increment to get to the next network.
            //    lastAssignedIp += maxHosts + 1; // Update lastAssignedIp to the last usable IP in this subnet.

            //    // Construct the network address string.
            //    string networkAddress = $"192.0.2.{nextNetworkIp}/{networkPrefix}";
            //    string firstHost = $"192.0.2.{nextNetworkIp + 1}/{networkPrefix}";
            //    string lastHost = $"192.0.2.{lastAssignedIp - 1}/{networkPrefix}";
            //    string broadcastAddress = $"192.0.2.{lastAssignedIp}/{networkPrefix}";

            //    var subnetResult = new CalculationResult
            //    {
            //        NetworkRange = networkAddress,
            //        BroadcastAddress = broadcastAddress,
            //        FirstHost = firstHost,
            //        LastHost = lastHost,
            //        SubnetMask = subnetMask
            //    };
            //    if (networkPrefix! > 8)
            //    {
            //        tempResults.Add((subnetResult, originalIndex));
            //    }
            //}
            var orderedResults = tempResults.OrderBy(item => item.originalIndex).Select(item => item.result).ToList();
            resultsList = orderedResults;
        }
        private void Reset()
        {
            inputValues.Clear();
            lastAssignedIp = -1;
            resultsList.Clear();
        }

       

        public SubnetData[] subnetData = new SubnetData[]
        {
            new SubnetData { SubnetMask = "255.0.0.0", BinaryAddress = "11111111.00000000.00000000.00000000", PrefixLength = 8, MaxHosts = (int)Math.Pow(2, 32 - 8) - 2 },
            new SubnetData { SubnetMask = "255.255.0.0", BinaryAddress = "11111111.11111111.00000000.00000000", PrefixLength = 16, MaxHosts = (int)Math.Pow(2, 32 - 16) - 2 },
            new SubnetData { SubnetMask = "255.255.255.0", BinaryAddress = "11111111.11111111.11111111.00000000", PrefixLength = 24, MaxHosts = (int)Math.Pow(2, 32 - 24) - 2 },
            new SubnetData { SubnetMask = "255.255.255.128", BinaryAddress = "11111111.11111111.11111111.10000000", PrefixLength = 25, MaxHosts = (int)Math.Pow(2, 32 - 25) - 2 },
            new SubnetData { SubnetMask = "255.255.255.192", BinaryAddress = "11111111.11111111.11111111.11000000", PrefixLength = 26, MaxHosts = (int)Math.Pow(2, 32 - 26) - 2 },
            new SubnetData { SubnetMask = "255.255.255.224", BinaryAddress = "11111111.11111111.11111111.11100000", PrefixLength = 27, MaxHosts = (int)Math.Pow(2, 32 - 27) - 2 },
            new SubnetData { SubnetMask = "255.255.255.240", BinaryAddress = "11111111.11111111.11111111.11110000", PrefixLength = 28, MaxHosts = (int)Math.Pow(2, 32 - 28) - 2 },
            new SubnetData { SubnetMask = "255.255.255.248", BinaryAddress = "11111111.11111111.11111111.11111000", PrefixLength = 29, MaxHosts = (int)Math.Pow(2, 32 - 29) - 2 },
            new SubnetData { SubnetMask = "255.255.255.252", BinaryAddress = "11111111.11111111.11111111.11111100", PrefixLength = 30, MaxHosts = (int)Math.Pow(2, 32 - 30) - 2 }
        };
    }

    public class CalculationResult
    {
        public string NetworkRange { get; set; }
        public string BroadcastAddress { get; set; }
        public string FirstHost { get; set; }
        public string LastHost { get; set; }
        public string SubnetMask { get; set; }
        public string NumberSubnets { get; set; }
    }

    public class SubnetData
    {
        public string SubnetMask { get; set; }
        public string BinaryAddress { get; set; }
        public int PrefixLength { get; set; }
        public int MaxHosts { get; set; }
    }
}
