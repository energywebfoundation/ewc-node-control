pragma solidity ^0.5.0;
pragma experimental ABIEncoderV2;

import "./NodeControlInterface.sol";

contract NodeControlSimple is NodeControlInterface {

    event UpdateAvailable(address _targetValidator);

    modifier onlyOwner{require(msg.sender == owner, "Error: Not owner");_;}

    address public owner;
    mapping (address => ValidatorState) public currentState;

    constructor() public {
        owner = msg.sender;
    }

    ///@notice Returns the current state of a validator
    ///@param _targetValidator The validator whos state you want
    ///@return The state of the validator
    function RetrieveUpdate(address _targetValidator) external view returns (ValidatorState memory) {
        return currentState[_targetValidator];
    }

    ///@notice sets the state for a validator and emits update event
    ///@param _targetValidator The validator whos state needs to be updated
    function updateValidator(address _targetValidator, bytes memory _dockerSha, string memory _dockerName, bytes memory _chainSpecSha, string memory _chainSpecUrl, bool _isSigning) public onlyOwner {
        require(!(sha256(bytes(currentState[_targetValidator].dockerSha)) == sha256(bytes(_dockerSha)) && sha256(bytes(currentState[_targetValidator].dockerName)) == sha256(bytes(_dockerName)) && sha256(bytes(currentState[_targetValidator].chainSpecSha)) == sha256(bytes(_chainSpecSha)) && sha256(bytes(currentState[_targetValidator].chainSpecUrl)) == sha256(bytes(_chainSpecUrl)) && currentState[_targetValidator].isSigning == _isSigning), "");
        
        currentState[_targetValidator].dockerSha = _dockerSha;
        currentState[_targetValidator].dockerName = _dockerName;
        currentState[_targetValidator].chainSpecSha = _chainSpecSha;
        currentState[_targetValidator].chainSpecUrl = _chainSpecUrl;
        currentState[_targetValidator].isSigning = _isSigning;
        currentState[_targetValidator].updateIntroduced = now;

        emit UpdateAvailable(_targetValidator);
    }

    ///@notice Lets the validator confirm the update
    function confirmUpdate() external {
        require(currentState[msg.sender].dockerSha.length != 0, "Error: You are not a validator!");
        currentState[msg.sender].updateConfirmed = now;
    }

    ///@notice Changes the owner of the NodeControlContract
    ///@param _newOwner The new owner of the contract. Can not be null.
    function setOwner(address _newOwner) public onlyOwner {
        require(_newOwner != address(0x0), "Error: New owner can not be null");
        owner = _newOwner;
    }
}