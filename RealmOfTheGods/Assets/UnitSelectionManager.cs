using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class UnitSelectionManager : MonoBehaviour {

    public UnitSelection[] unitSelections;
    public static UnitSelection[] unitSelectionsStatic;

	// Use this for initialization
	void Start () {
        unitSelectionsStatic = unitSelections;
        //TurnOffUnitSelections();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void TurnOffUnitSelections(UnitSelection exception) {
        foreach (UnitSelection unitSelection in unitSelectionsStatic) {
            if (unitSelection != exception) {
                unitSelection.frontCard.SetActive(false);
                unitSelection.gameObject.SetActive(false);
            }
        }
    }

    public static void TurnOnUnitSelection(TeamType teamType) {
        foreach(UnitSelection unitSelection in unitSelectionsStatic) {
            if(unitSelection.team == teamType) {
                unitSelection.gameObject.SetActive(true);
                //unitSelection.frontCard.SetActive(true);
                
            }
            else {/*
                unitSelection.frontCard.SetActive(false);
                unitSelection.vbbCancel.gameObject.SetActive(false);
                unitSelection.vbbMove.gameObject.SetActive(false);
                unitSelection.vbbBoost.gameObject.SetActive(false);*/
                unitSelection.frontCard.GetComponent<ImageTargetBehaviour>().enabled = false;
            }
        }
    }

    public static void TurnLaserOff(GameObject goLaser) {
        foreach (UnitSelection unitSelection in unitSelectionsStatic) {
            if(unitSelection.frontCard == goLaser) {
                if (unitSelection.controlOption == UnitSelection.ControlOption.Move) {
                    unitSelection.SetLineActive(false);
                }
            }
        }
    }

    public static void TurnLaserOn(GameObject goLaser) {
        foreach (UnitSelection unitSelection in unitSelectionsStatic) {
            if (unitSelection.frontCard == goLaser) {
                if (unitSelection.controlOption == UnitSelection.ControlOption.Move) {
                    unitSelection.SetLineActive(true);
                }
            }
        }
    }
}
