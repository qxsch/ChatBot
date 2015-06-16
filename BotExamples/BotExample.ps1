try
{
    <#$refs = @( "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\Microsoft.CSharp.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Core.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.DataSetExtensions.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\WindowsPowerShell\3.0\System.Management.Automation.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Speech.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Xml.dll",
    "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Xml.Linq.dll" )
    Add-Type -Path "bin\ReleaseWithoutLync\ChatBot.dll" -ReferencedAssemblies $refs #>

    Add-Type -Path "bin\ReleaseWithoutLync\ChatBot.dll"
}
catch
{
    $_.LoaderExceptions | %
    {
        Write-Error $_.Message
    }
}




$ruleGenerator = New-Object -TypeName QXS.ChatBot.ChatBotRuleGenerator

<#
# PARSE FROM STRING:
$xmlstring = ""
Get-Content ".\ExampleRules.xml" | foreach { $xmlstring += "$_`n" } 
$chatbot = New-Object QXS.ChatBot.ChatBot $ruleGenerator.Parse($xmlstring) 
#>

# PARSE FROM FILE:
$chatbot = New-Object QXS.ChatBot.ChatBot  $ruleGenerator.ParseFromFile($PSScriptRoot + "\ExampleRules.xml")
$consoleChatSession = New-Object QXS.ChatBot.ConsoleChatSession
$chatbot.talkWith($consoleChatSession)