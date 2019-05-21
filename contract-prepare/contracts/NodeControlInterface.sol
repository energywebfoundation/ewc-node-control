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
    event UpdateAvailable(address indexed targetValidator);
    function retrieveExpectedState(address _targetValidator) external view returns (ValidatorState memory);
    function confirmUpdate() external;
}