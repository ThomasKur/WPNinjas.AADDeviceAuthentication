# WPNinjas Password Generation


I developed this helper library because I require often secure generated passwords. This was especially created to support [CloudLAPS](https://github.com/MSEndpointMgr/CloudLAPS).
Improvements and contributions are always welcome.

## Nuget package for .NET projects
The package is available on [Nuget](https://www.nuget.org/packages/WPNinjas.PasswordGeneration). Simply install it by searching for 'WPNinjas.PasswordGeneration'. You can the retrieve generated passwords by using:

```c#

using System;
using WPNinjas.PasswordGeneration;

namespace WPNinjas.PasswordGeneration.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String pw = PwService.GetRandomPassword(20,null);
                Console.WriteLine($"Successfully generated PW '{pw}'");
            } catch (Exception ex)
            {
                Console.WriteLine("Error occured.");
                Console.WriteLine(ex.Message);

            }

        }
    }
}

```

## PowerShell Module
For your convinience the module is published to the [Powershell Gallery](https://www.powershellgallery.com/packages/WPNinjas.PasswordGeneration/).

```powershell
Install-Module -Name WPNinjas.PasswordGeneration

# Various options to retrieve the password

Invoke-PasswordGeneration 
Invoke-SecurePasswordGeneration 
Invoke-PasswordGeneration -Length 8
Invoke-SecurePasswordGeneration -Length 8

Invoke-PasswordGeneration -AllowedCharacters "ABCDEFGH1234567890"
Invoke-SecurePasswordGeneration -AllowedCharacters "ABCDEFGH1234567890"

Invoke-PasswordGeneration -AllowedCharacters "ABCDEFGH1234567890" -Length 8
Invoke-SecurePasswordGeneration -AllowedCharacters "ABCDEFGH1234567890" -Length 8

```

