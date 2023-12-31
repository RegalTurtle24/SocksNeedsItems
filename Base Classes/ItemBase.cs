﻿using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2GenericModTemplate1.Utils;
using UnityEngine;
using SocksNeedsItems;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace RoR2GenericModTemplate1.Base_Classes
{
    public abstract class ItemBase
    {
        //sets item name
        public abstract string ItemName { get; }
        //sets lang token
        public abstract string ItemLangTokenName { get; }
        //sets the pickup description
        public abstract string ItemPickupDesc { get; }
        //sets the full description
        public abstract string ItemFullDescription { get; }
        //sets logbook lore
        public abstract string ItemLore { get; }

        //sets item tier
        public abstract ItemTier Tier { get; }
        //sets item tags. See ItemTag in a tool like dnSpy to learn more.
        public virtual ItemTag[] ItemTags { get; } = { };

        //sets paths for model and icons
        public abstract string ItemModelPath { get; }
        public abstract string ItemIconPath { get; }

        //determines whether item can be removed
        public virtual bool CanRemove { get; }
        //determines if item is hidden from the game
        public virtual bool Hidden { get; }

        //determines whether or not an unlockable is required
        public virtual bool HasUnlockable { get; }

        //creates an unlockable requirement
        public virtual UnlockableDef ItemPreReq { get; }

        //creates necessary GameObject field for display rules
        public static GameObject ItemBodyModelPrefab;

        //initializes the item
        public abstract void Init(ConfigFile config);

        public abstract void CreateConfig(ConfigFile config);

        //actually creates instance of item
        /*public ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();*/
        public ItemDef itemDef { get; set; }

        //sets the lang tokens for in game use
        protected void CreateLang()
        {

            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDesc);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_DESCRIPTION", ItemFullDescription);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_LORE", ItemLore);

        }

        //sets display rules
        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        //actually defines the item
        protected void CreateItem()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = "ITEM_" + ItemLangTokenName;
            itemDef.nameToken = "ITEM_" + ItemLangTokenName + "_NAME";
            itemDef.pickupToken = "ITEM_" + ItemLangTokenName + "_NAME";
            itemDef.descriptionToken = "ITEM_" + ItemLangTokenName + "_DESCRIPTION";
            itemDef.loreToken = "ITEM_" + ItemLangTokenName + "_LORE";
            if (ItemModelPath == "null")
            {
                itemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();
            } else
            {
                itemDef.pickupModelPrefab = Main.Assets.LoadAsset<GameObject>(ItemModelPath);
            }
            if (ItemIconPath == "null")
            {
                itemDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            } else
            {
                itemDef.pickupIconSprite = Main.Assets.LoadAsset<Sprite>(ItemIconPath);
            }
            itemDef.hidden = Hidden;
            itemDef.tags = ItemTags;
            itemDef.canRemove = CanRemove;
            itemDef.tier = Tier;
            itemDef.deprecatedTier = Tier;
            itemDef.unlockableDef = ItemPreReq;

            //sets the display of the item on characters
            /*var itemDisplayRuleDict = CreateItemDisplayRules();*/

            //adds our item to the game via ItemAPI
            /*ItemAPI.Add(new CustomItem(itemDef, CreateItemDisplayRules()));*/
            ItemAPI.Add(new CustomItem(itemDef, new ItemDisplayRuleDict(null)));

        }

        //where hooks go
        public abstract void Hooks();

        //gets count of item from CharacterBody or CharacterMaster
        public int GetCount(CharacterBody body)
        {

            if (!body || !body.inventory)
            {

                return 0;

            }

            return body.inventory.GetItemCount(itemDef);

        }

        public int GetCount(CharacterMaster master)
        {

            if (!master || !master.inventory)
            {

                return 0;

            }

            return master.inventory.GetItemCount(itemDef);

        }

    }

}