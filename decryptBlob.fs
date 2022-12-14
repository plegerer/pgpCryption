namespace Company.Function
open System
open System.IO
open Microsoft.Azure.WebJobs
open Microsoft.Extensions.Logging
open PgpCore

module decryptBlob =

    [<FunctionName("decryptBlob")>]
    let run ([<BlobTrigger("cryption-pgp/encrypted-in/{name}.pgp", Connection = "BlobConnection")>] inputBlob: Stream, name: string)
            ([<Blob("cryption-pgp/decrypted-out/{name}", FileAccess.Write, Connection = "BlobConnection")>] outputBlob: Stream)
            (log: ILogger) =

        use publicKey = new MemoryStream(
            Environment.GetEnvironmentVariable "pgp-private-key" 
            |> Convert.FromBase64String)

        let encryptionKeys = new EncryptionKeys(publicKey,"Ti81CP250.y_d3a!s")    
        use pgp = new PGP(encryptionKeys)

        let _ = pgp.DecryptStream(inputBlob,outputBlob)

        let msg = sprintf "F# Blob trigger function Processed blob\nName: %s \n Size: %d Bytes" name inputBlob.Length
        log.LogInformation msg
