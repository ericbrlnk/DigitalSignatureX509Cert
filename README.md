# DigitalSignatureX509Cert 

## Description

Solution consists of two projects: DigitalSignatureX509Sign (for signing XML-document with the private key) 
and DigitalSignatureX509Validate (verifying signed XML-document).

## Compiling the code

- Project that targets .NET Framework, System.Security.dll should be included
- In a project that targets .NET or .NET 5, install NuGet package System.Security.Cryptography.Xml
- Include following namespaces: System.Xml, System.Security.Cryptography, and System.Security.Cryptography.Xml

## .NET Security

Private key of an asymmetric key should not be stored or transferred in a plaintext.
Private keys should not be embedded in the source code. 
One of the reasons is, that it can be disassembled with Ildasm.exe - IL Disassembler.

