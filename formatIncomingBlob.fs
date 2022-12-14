namespace Company.Function
open System
open System.Text
open System.IO
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host
open Microsoft.Extensions.Logging
open Azure.Storage.Blobs

module formatIncomingBlob =


    [<FunctionName("formatIncomingBlob")>]
    let run ([<BlobTrigger("cryption-pgp/decrypted-out/{name}", Connection = "BlobConnection")>] inputBlob: Stream, name: string)
            ([<Blob("cryption-pgp", Connection = "BlobConnection")>] outputContainer: BlobContainerClient)
            (log: ILogger) =
            
        let readLines = seq{
                let sr = new StreamReader(inputBlob)
                while not sr.EndOfStream do
                    yield sr.ReadLine () 
                }

        let readList = readLines |> Seq.toList

        let firstLine = readList |> List.tryHead
        
        let lineCount (fl:string) = 
            let flsplit = fl.Split '|' 
            match flsplit|>Array.length with
            | 5 -> match Int32.TryParse flsplit[2] with
                    | false, _ -> None
                    | true, v -> Some v
            | _ -> None

        let r =
            match firstLine |> Option.bind lineCount with
            | Some 0 -> ()
            | Some v when v > 0  ->
                let blockBlob = outputContainer.GetBlobClient("formatted-out/"+name)    
                use str = blockBlob.OpenWrite(true,new Models.BlobOpenWriteOptions())
                use writer = new StreamWriter(str)
                readList   
                |> List.iteri (fun i line ->
                                                match i with
                                                | 0 -> ()
                                                | x when x < v -> writer.WriteLine(line)
                                                | x when x = v -> writer.Write(line)
                                                | _ -> () )
            | _ -> ()        


        let msg = sprintf "F# Blob trigger function Processed blob\nName: %s \n Size: %d Bytes.\n First Line is %A. \n Line Count is %A" name inputBlob.Length firstLine (firstLine  |> Option.bind lineCount)
        log.LogInformation msg
