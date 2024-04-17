
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System.Text.RegularExpressions;
using Dalamud.Plugin;
using System.Collections.Generic;



namespace betterroll;

public class MainWindow : Window, IDisposable
{

    private bool isListening = false;
    private Dictionary<string, int> rollResults = new Dictionary<string, int>();
    private int selectedOption = 0;
    private int specifiedNumber = 0;
    private HashSet<XivChatType> excludedChatTypes = new HashSet<XivChatType>
{
    XivChatType.Debug,
    XivChatType.Urgent,
    XivChatType.Notice,
    XivChatType.Say,
    XivChatType.Shout,
    XivChatType.TellOutgoing,
    XivChatType.TellIncoming,
    XivChatType.Party,
    XivChatType.Alliance,
    XivChatType.Ls1,
    XivChatType.Ls2,
    XivChatType.Ls3,
    XivChatType.Ls4,
    XivChatType.Ls5,
    XivChatType.Ls6,
    XivChatType.Ls7,
    XivChatType.Ls8,
    XivChatType.FreeCompany,
    XivChatType.NoviceNetwork,
    XivChatType.CustomEmote,
    XivChatType.StandardEmote,
    XivChatType.Yell,
    XivChatType.CrossParty,
    XivChatType.PvPTeam,
    XivChatType.CrossLinkShell1,
    XivChatType.Echo,
    XivChatType.SystemError,
    XivChatType.SystemMessage,
    XivChatType.GatheringSystemMessage,
    XivChatType.ErrorMessage,
    XivChatType.NPCDialogue,
    XivChatType.NPCDialogueAnnouncements,
    XivChatType.RetainerSale,
    XivChatType.CrossLinkShell2,
    XivChatType.CrossLinkShell3,
    XivChatType.CrossLinkShell4,
    XivChatType.CrossLinkShell5,
    XivChatType.CrossLinkShell6,
    XivChatType.CrossLinkShell7,
    XivChatType.CrossLinkShell8
};









    public MainWindow(DalamudPluginInterface pluginInterface) : base("BetterRoll by:Cindy-Master 闲鱼司马")
    {
        Service.ChatGui.ChatMessage += OnChatMessage;
    }

    private void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!isListening || excludedChatTypes.Contains(type))
            return;

        var messageText = message.TextValue;

        var regexPattern = new Regex(
            @"(?:ダイス！ (.+?)は、(\d+)を出した！|Random! (.+?) rolls a (\d+)(?!.*out of)|(.+?)掷出了(\d+)点！(?!.*（最大\d+）)|(.+?) würfelt eine (\d+)(?!.*-seitigen)|(.+?) jette les dés et obtient (\d+)(?!.*Lancer d'un dé))"
        );
        var match = regexPattern.Match(messageText);

        if (match.Success)
        {

            string name = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Success ? match.Groups[3].Value : match.Groups[5].Success ? match.Groups[5].Value : match.Groups[7].Success ? match.Groups[7].Value : match.Groups[9].Value;
            int roll = int.Parse(match.Groups[2].Success ? match.Groups[2].Value : match.Groups[4].Success ? match.Groups[4].Value : match.Groups[6].Success ? match.Groups[6].Value : match.Groups[8].Success ? match.Groups[8].Value : match.Groups[10].Value);

            if (!rollResults.ContainsKey(name))
            {
                rollResults[name] = roll;
            }
        }
    }









    public void Dispose()
    {
        Service.ChatGui.ChatMessage -= OnChatMessage;
    }



    public unsafe override void PreOpenCheck()
    {
        IsOpen = true;
    }
    public unsafe override void Draw()
    {


        if (ImGui.Checkbox("监听消息", ref isListening))
        {
            if (!isListening)
            {

                rollResults.Clear();
            }
        }
        ImGui.Text("只会记录每个人第一次的Roll点记录");
        ImGui.Text("如需重新记录请重新勾选监听");


        ImGui.RadioButton("最大值", ref selectedOption, 0); ImGui.SameLine();
        ImGui.RadioButton("最小值", ref selectedOption, 1); ImGui.SameLine();
        ImGui.RadioButton("指定数值(找出最接近的指定数的Roll点)", ref selectedOption, 2);

        if (selectedOption == 2)
        {
            ImGui.InputInt("指定数值", ref specifiedNumber);
        }

        string highlightName = null;
        int highlightRoll = (selectedOption == 1) ? int.MaxValue : int.MinValue;
        foreach (var result in rollResults)
        {
            switch (selectedOption)
            {
                case 0:
                    if (result.Value > highlightRoll)
                    {
                        highlightName = result.Key;
                        highlightRoll = result.Value;
                    }
                    break;
                case 1:
                    if (result.Value < highlightRoll)
                    {
                        highlightName = result.Key;
                        highlightRoll = result.Value;
                    }
                    break;
                case 2:
                    if (Math.Abs(result.Value - specifiedNumber) < Math.Abs(highlightRoll - specifiedNumber))
                    {
                        highlightName = result.Key;
                        highlightRoll = result.Value;
                    }
                    break;
            }
        }
        foreach (var result in rollResults)
        {
            if (result.Key == highlightName)
            {
                ImGui.TextColored(new Vector4(1, 0, 0, 1), $"{result.Key} 掷出了 {result.Value} 点");
            }
            else
            {
                ImGui.Text($"{result.Key} 掷出了 {result.Value} 点");
            }
        }

    }



}
