$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
import-module "$scriptPath\WPNinjas.PasswordGeneration\WPNinjas.PasswordGeneration.psm1" -force

Invoke-PasswordGeneration 
Invoke-SecurePasswordGeneration 
Invoke-PasswordGeneration -Length 8
Invoke-SecurePasswordGeneration -Length 8

Invoke-PasswordGeneration -AllowedCharacters "ABCDEFGH1234567890"
Invoke-SecurePasswordGeneration -AllowedCharacters "ABCDEFGH1234567890"

Invoke-PasswordGeneration -AllowedCharacters "ABCDEFGH1234567890" -Length 8
Invoke-SecurePasswordGeneration -AllowedCharacters "ABCDEFGH1234567890" -Length 8