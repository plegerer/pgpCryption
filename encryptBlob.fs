namespace Company.Function
open System
open System.IO
open Microsoft.Azure.WebJobs
open Microsoft.Extensions.Logging
open PgpCore

module encryptBlob =

    

    [<FunctionName("encryptBlob")>]
    let run ([<BlobTrigger("cryption-pgp/decrypted-in/{name}", Connection = "BlobConnection")>] inputBlob: Stream, name: string)
            ([<Blob("cryption-pgp/encrypted-out/{name}.pgp", FileAccess.Write, Connection = "BlobConnection")>] outputBlob: Stream) 
            (log: ILogger) =
        
        use publicKey = new MemoryStream(
            Environment.GetEnvironmentVariable "pgp-public-key" 
            |> Convert.FromBase64String)

        let encryptionKeys = new EncryptionKeys(publicKey)
        use pgp = new PGP(encryptionKeys)

        let _ = pgp.EncryptStream(inputBlob,outputBlob)

        let msg = sprintf "F# Blob trigger function Processed blob\nName: %s \n Size: %d Bytes" name inputBlob.Length
        log.LogInformation msg
