Function Get-AADDeviceToken(){
    <#
    .DESCRIPTION
    This method will return a AAD Device Token to identitfy the device and ensure the content was not modified during transfer.

    .EXAMPLE
    $token = Get-AADDeviceToken -Content "Test123"


    .PARAMETER Content
        The content will be used to generate the signature.

    .NOTES
    Author: Thomas Kurth/baseVISION
    Date:   30.1.2022

    History
        See Release Notes in Github.

    #>
    [CmdletBinding()]
    [OutputType([String])]
    Param(
        [Parameter(Mandatory=$false)]
        [String]$Content
    )
      

    #region Initialization
    ########################################################
    Write-Log "Start Script $Scriptname"
    #endregion

    #region Main Script
    ########################################################
    if(-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)){
        throw "Not executed as Administrator. Please start PowerShell with Administrative Privileges."
    }
    $client = New-Object WPNinjas.AADDeviceAuthentication.Client.AADDeviceAuthClient
    $client.GetToken($content)
    
    #endregion
    #region Finishing
    ########################################################

    Write-Log "End Script $Scriptname"
    #endregion
}