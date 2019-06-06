namespace src.Models
{

    /// <summary>
    /// Model that described a state change
    /// </summary>
    public class StateChangeAction
    {
        /// <summary>
        /// Selects the update path. see UpdateMode
        /// </summary>
        public UpdateMode Mode { get; set; } = UpdateMode.Unknown;

        /// <summary>
        /// Transports the payload for the given update mode.
        /// <list type="table">
        /// <item>
        /// <term>Docker Mode</term>
        /// <description>Docker image (eg. parity/parity:v2.3.4)</description>
        /// </item>
        /// <item>
        /// <term>Chainspec Mode</term>
        /// <description>URL to the new chainspec file</description>
        /// </item>
        /// </list>
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// Transports the hash for the payload
        /// <list type="table">
        /// <item>
        /// <term>Docker Mode</term>
        /// <description>the hash of the docker image represented by the image id</description>
        /// </item>
        /// <item>
        /// <term>Chainspec Mode</term>
        /// <description>SHA256 hash of the file contents</description>
        /// </item>
        /// </list>
        /// </summary>
        public string PayloadHash { get; set; } = string.Empty;
    }
}