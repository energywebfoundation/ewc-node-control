pragma solidity ^0.5.0;
pragma experimental ABIEncoderV2;

interface NodeControlInterface {
    struct ValidatorState {
        bytes dockerSha;
        string dockerName;
        bytes chainSpecSha;
        string chainSpecUrl;
        bool isSigning;
        uint updateIntroduced;
        uint updateConfirmed;
    }
    event UpdateAvailable(address targetValidator, uint eventId);
    function RetrieveUpdate(address _targetValidator) external view returns (ValidatorState memory);
    function confirmUpdate() external;
}