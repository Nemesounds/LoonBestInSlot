﻿using AddonManager.Models;
using AngleSharp.Html.Dom;
using static System.Net.WebRequestMethods;

namespace AddonManager.Importers;

public class EmblemImporter : LootImporter
{
    private List<string> wowheadUriList = new List<string>
    {
        //@"https://www.wowhead.com/wotlk/npc=31580/arcanist-ivrenne",
        //@"https://www.wowhead.com/wotlk/npc=31580/arcanist-ivrenne#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=31580/arcanist-ivrenne#sells;100",
        //@"https://www.wowhead.com/wotlk/npc=31582/magistrix-lambriesse",
        //@"https://www.wowhead.com/wotlk/npc=31582/magistrix-lambriesse#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=31582/magistrix-lambriesse#sells;100",
        //@"https://www.wowhead.com/wotlk/npc=31579/arcanist-adurin",
        //@"https://www.wowhead.com/wotlk/npc=31579/arcanist-adurin#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=31579/arcanist-adurin#sells;100",
        //@"https://www.wowhead.com/wotlk/npc=31581/magister-brasael",
        //@"https://www.wowhead.com/wotlk/npc=31581/magister-brasael#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=31581/magister-brasael#sells;100",
        //@"https://www.wowhead.com/wotlk/npc=33964/arcanist-firael",
        //@"https://www.wowhead.com/wotlk/npc=33964/arcanist-firael#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=33964/arcanist-firael#sells;100",
        //@"https://www.wowhead.com/wotlk/npc=33963/magister-sarien",
        //@"https://www.wowhead.com/wotlk/npc=33963/magister-sarien#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=33963/magister-sarien#sells;100",
        //@"https://www.wowhead.com/wotlk/npc=35494/arcanist-miluria",
        //@"https://www.wowhead.com/wotlk/npc=35494/arcanist-miluria#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=35574/magistrix-iruvia",
        //@"https://www.wowhead.com/wotlk/npc=35574/magistrix-iruvia#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=35573/arcanist-asarina",
        //@"https://www.wowhead.com/wotlk/npc=35573/arcanist-asarina#sells;50",
        //@"https://www.wowhead.com/wotlk/npc=35579/aspirant-forudir",
        //@"https://www.wowhead.com/wotlk/npc=35579/aspirant-forudir#sells;50",
        @"https://www.wowhead.com/wotlk/npc=35577/valiant-laradia",
        @"https://www.wowhead.com/wotlk/npc=35577/valiant-laradia#sells;50",
        @"https://www.wowhead.com/wotlk/npc=35580/aspirant-naradiel",
        @"https://www.wowhead.com/wotlk/npc=35580/aspirant-naradiel#sells;50",
        @"https://www.wowhead.com/wotlk/npc=35578/valiant-bressia",
        @"https://www.wowhead.com/wotlk/npc=35578/valiant-bressia#sells;50",
        @"https://www.wowhead.com/wotlk/npc=35496/rueben-lauren",
        @"https://www.wowhead.com/wotlk/npc=35496/rueben-lauren#sells;50",
        @"https://www.wowhead.com/wotlk/npc=207128/animated-constellation",
        @"https://www.wowhead.com/wotlk/npc=207128/animated-constellation#sells;50"
    };

    internal override string FileName { get => "EmblemItemList"; }

    internal override async Task<DatabaseItems> InnerConvert(DatabaseItems items, Action<string> writeToLog)
    {
        items.Items.Clear();

        await Common.ReadWowheadItemList(wowheadUriList, (row, itemId, itemName) =>
        {
            var success = false;
            var currencySource = "";
            var currencyNumber = "";
            var currencySourceLocation = "";

            Common.RecursiveBoxSearch(row.Children[10], (anchorObject) =>
            {
                var item = ((IHtmlAnchorElement)anchorObject).PathName.Replace("/wotlk", "").Replace("/currency=", "").Replace("/item=", "");

                var currencyIdIndex = item.IndexOf("/");
                if (currencyIdIndex == -1)
                    currencyIdIndex = item.IndexOf("&");

                if (currencyIdIndex > -1)
                {
                    item = item.Substring(0, currencyIdIndex);

                    success = int.TryParse(item, out var currencyInteger);

                    if (success)
                    {
                        var sourceText = item == "101" ? "Emblem of Heroism" :
                            item == "102" ? "Emblem of Valor" :
                            item == "221" ? "Emblem of Conquest" :
                            item == "301" ? "Emblem of Triumph" :
                            item == "341" ? "Emblem of Frost" : 
                            item == "2589" ? "Sidereal Essence" :
                            item == "47247" ? "Trophy of the Crusade" : "unknown";

                        if (string.IsNullOrWhiteSpace(currencySource))
                            currencySource = sourceText;
                        else
                            currencySource = $"{currencySource} & {sourceText}";

                        if (string.IsNullOrWhiteSpace(currencyNumber))
                            currencyNumber = anchorObject.TextContent;
                        else
                            currencyNumber = $"{currencyNumber} & {anchorObject.TextContent}";

                        currencySourceLocation = "Emblem Vendor";
                    }
                }
                return success;
            });

            if (items.Items.ContainsKey(itemId))
            {
                items.Items.Remove(itemId);
            }
            var successfulAdd = items.Items.TryAdd(itemId, new DatabaseItem
            {
                Name = itemName,
                SourceNumber = currencyNumber,
                Source = currencySource,
                SourceLocation = currencySourceLocation,
                SourceType = "Dungeon Token"
            });
        }, writeToLog);

        return items;
    }
}
