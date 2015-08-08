using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace KSP_rusty
{
    class GUIInventory : GUIWindow
    {
        public GUIInventory()
        {
            base.setTitle("Inventory");
            base.minWidth = 500.0f;
            base.closeButton = false;
        }

        public override void drawWindow(int windowID)
        {
            GUIStyle mySty = new GUIStyle(GUI.skin.window);
            mySty.normal.textColor = mySty.focused.textColor = Color.white;
            mySty.hover.textColor = mySty.active.textColor = Color.yellow;
            mySty.onNormal.textColor = mySty.onFocused.textColor = mySty.onHover.textColor = mySty.onActive.textColor = Color.green;
            mySty.padding = new RectOffset(8, 8, 8, 8);

            GUILayout.BeginVertical();

            GUILayout.Label("Select an assembly");

            if (GUILayout.Button("StorePart") && EditorLogic.SelectedPart != null)
            {
                EditorLogic editor = EditorLogic.fetch;
                ShipConstruct ship = new ShipConstruct();

                Utils.addPartToShipRecursive(ship, EditorLogic.SelectedPart);

                ship.SaveShip().Save("D:/rusty.cfg");
				ShipConstruction.CaptureThumbnail(ship, "rusty_thumbs", "rusty");

				// store thumb
                KSP_rusty.Inventory.Add(ship.SaveShip());

                // root or not?
                if (EditorLogic.SelectedPart == EditorLogic.RootPart)
                {
                    KerbalGUIManager.print("[rusty] GUIInventory:StorePart: is root");
                    EditorLogic.DeletePart(EditorLogic.SelectedPart);
                }
                else
                {
                    KerbalGUIManager.print("[rusty] GUIInventory:StorePart: is not root");
                    EditorLogic.DeletePart(EditorLogic.SelectedPart);
                }
            }

            /*
            if (GUILayout.Button("Store"))
            {
                try
                {
                    EditorLogic editor = EditorLogic.fetch;
                    ShipConstruct ship = new ShipConstruct(editor.ship.shipName, editor.ship.shipFacility, editor.getSortedShipList());

                    KerbalGUIManager.print("[rusty] GUIInventory: storing");
                    
                    try
                    {
                        inv = new ShipConstruct();
                        if (inv.LoadShip(ship.SaveShip()))
                        {
                            KerbalGUIManager.print("[rusty] GUIInventory: load OK");
                        }
                        else
                        {
                            KerbalGUIManager.print("[rusty] GUIInventory: load failed");
                        }
                    }
                    catch (Exception e)
                    {
                        KerbalGUIManager.print("Exception GUIInventory:drawWindow():Store;saveShip: " + e.Message + "\n" + e.StackTrace);
                    }

                    KerbalGUIManager.print("[rusty] GUIInventory: deleting old");
                }
                catch (Exception e)
                {
                    KerbalGUIManager.print("Exception GUIInventory:drawWindow():Store: " + e.Message + "\n" + e.StackTrace);
                }
            }
             */
			
            if (GUILayout.Button("Load rusty.cfg"))
            {
                ConfigNode rusty2 = ConfigNode.Load("D:/rusty.cfg");

                EditorLogic editor = null;
                ShipConstruct ship = null;


                editor = EditorLogic.fetch;
                //editor.SpawnPart(PartLoader.getPartInfoByName("mk1pod"));
                //editor.SpawnConstruct(inv);

                // fix type
                rusty2.SetValue("type", "VAB", true);
                rusty2.Save("D:/rusty2.cfg");

                ship = new ShipConstruct();
                ship.LoadShip(rusty2);
            }
			
            if (KSP_rusty.Inventory != null)
            {
                foreach (ConfigNode inventoryItem in KSP_rusty.Inventory) {
                    string title = "";
                    int nrOfParts = 0;

                    try
                    {
                        // get number of parts
                        ConfigNode[] partNodes = inventoryItem.GetNodes("PART");
                        if (partNodes != null)
                        {
                            nrOfParts = partNodes.GetUpperBound(0) + 1;
                        }

                        // get vessel name
                        title = inventoryItem.GetValue("ship");
                        if (title != null && title.Length > 0)
                        {
                            title = "Load " + title + " (" + nrOfParts + ")";
                        }
                        else
                        {
                            title = "Load (" + nrOfParts + " parts)";
                        }
                    }
                    catch (Exception e)
                    {
                        KerbalGUIManager.print("Exception GUIInventory:drawWindow():Button title " + e.Message + "\n" + e.StackTrace);
                    }

                    if (GUILayout.Button(title))
                    {
                        KerbalGUIManager.print("[rusty] Adding part from inventory");

                        EditorLogic editor = null;
                        ShipConstruct ship = null;
                        try
                        {
                            editor = EditorLogic.fetch;
							ship = new ShipConstruct();
							ship = rustyInventory.buildConstruct(inventoryItem);
							ship.SaveShip().Save("D:/rusty.cfg");
							editor.SpawnConstruct(ship);

							/*
                            //editor.SpawnPart(PartLoader.getPartInfoByName("mk1pod"));
                            //editor.SpawnConstruct(inv);
                            
                            // fix type
                            inventoryItem.SetValue("type", "VAB", true);

                            inventoryItem.Save("D:/rusty.cfg");

                            ship = new ShipConstruct();
                            ship.LoadShip(inventoryItem);
                            // TODO: editor.OnPodSpawn(PartLoader.getPartInfoByName("mk1pod"));
                            */
                            KSP_rusty.Inventory.Remove(inventoryItem);
                            break;
                        }
                        catch (Exception e)
                        {
                            if (editor == null) KerbalGUIManager.print("[rusty] editor is null");
                            if (ship == null) KerbalGUIManager.print("[rusty] ship is null");
                            KerbalGUIManager.print("[rusty] Exception GUIInventory:drawWindow():Load " + e.Message + "\n" + e.StackTrace);
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }
    }
}
