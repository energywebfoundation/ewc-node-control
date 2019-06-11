namespace src.Models
{
    /// <summary>
    /// Defines the update that should be carried out
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Unknown/Unsupported
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Update the parity docker image
        /// </summary>
        Docker = 1,
        /// <summary>
        /// Update the chain spec file
        /// </summary>
        ChainSpec = 2,

        /// <summary>
        /// Enable/Disable signing of blocks
        /// </summary>
        ToggleSigning = 3
    }
}