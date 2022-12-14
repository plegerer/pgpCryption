# gpgCryption: BlobTrigger - FSharp

This is a simple Azure-function to encode files dropped in a blob-container with a pgp pbublic keys by dropping the encoded file in a second container

## Learn more

<TODO> Documentation

The pgp public key must be provided as an environment variable encoded base64. An easy way to encode the keyfile is using Powershell:
```
PS> $filecontent = get-content pathto/key_public.asc -raw
PS> $filecontentinbytes =[System.Text.Encoding]::UTF8.GetBytes($filecontent)
PS> $encoded= [System.Convert]::ToBase64String($filecontentinbytes)
```