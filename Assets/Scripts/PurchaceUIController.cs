using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DapperLabs.Flow.Sdk;
using DapperLabs.Flow.Sdk.DataObjects;
using DapperLabs.Flow.Sdk.Cadence;
using DapperLabs.Flow.Sdk.DevWallet;

public class PurchaceUIController : MonoBehaviour
{
    // Start is called before the first frame update
    public Button quitButton;
    public Button createButton;
    public VisualElement buyGUI;
    public VisualElement root;
    public string address;
    public void UIStart()
    {
        //get gameObject by tag only one
        root = GameObject.FindWithTag("UIDocument").GetComponent<UIDocument>().rootVisualElement;

        quitButton = root.Q<Button>("quit");
        createButton = root.Q<Button>("createAccount");

        buyGUI = root.Q<VisualElement>("buyGUI");
        buyGUI.style.display = DisplayStyle.Flex;

        address = GetComponent<PlayerAccountInit>().playerName;

        getInfo();

        quitButton.clicked += QuitButtonPressed;
        createButton.clicked += CreateButtonPressed;
    }

    void QuitButtonPressed()
    {
        buyGUI.style.display = DisplayStyle.None;
    }

    private async void getInfo()
    {
        //get playername
        string script = @"
import BasicInventory from 0x91c7fd71eaa35f2f
pub fun main(): Bool {
  return BasicInventory.inDatabase(_address: "+address+@");
}
 ";
        FlowScriptRequest scriptReq = new FlowScriptRequest
        {
            Script = script
        };

        FlowScriptResponse response = await Scripts.ExecuteAtLatestBlock(scriptReq);

        CadenceBool responseStr = (CadenceBool)response.Value;
        if(responseStr.Value == false)
        {
            root.Q<VisualElement>("createContainer").style.display = DisplayStyle.Flex;
        }
        else
        {
            root.Q<VisualElement>("authorizedContainer").style.display = DisplayStyle.Flex;
            getInitialInventory();
        }
            
    }

    private async void CreateButtonPressed()
    {
            string script = @"
import BasicInventory from 0x91c7fd71eaa35f2f
transaction {
  prepare(acct: AuthAccount) {}
  execute {
    log(BasicInventory.initializeAccount(_address:"+address+@"))
  }
}
";

        // Using the same account as proposer, payer and authorizer. 
        FlowTransactionResponse response = await Transactions.Submit(script);
        if (response.Error != null)
        {
            Debug.LogError(response.Error.Message);
            return;
        }
        root.Q<VisualElement>("createContainer").style.display = DisplayStyle.Flex;
        getInfo();
    }

    private async void getInitialInventory()
    {
        string script = @"
import BasicInventory from 0x91c7fd71eaa35f2f
pub fun main(): [Bool]? {
  return BasicInventory.getItemsList(_address: "+address+@");
}
        ";
        FlowScriptRequest scriptReq = new FlowScriptRequest
        {
            Script = script
        };

        FlowScriptResponse response = await Scripts.ExecuteAtLatestBlock(scriptReq);

        Debug.Log(response.Value);
        // The script returns an array of ids, so convert the return value to an array. 
        // CadenceArray responseVal = (CadenceArray)response.Value;

        // string resultText = $"Account {address} NFTs:\n";

        // // Iterate the array of UInt64 values
        // foreach (CadenceBase value in responseVal.Value)
        // {
        //     CadenceNumber nftId = (CadenceNumber)value;
        //     resultText += $"{nftId.Value}\n";
        // }
        // Debug.Log(resultText);

    }
}
