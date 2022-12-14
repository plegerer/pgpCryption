#r "nuget: Azure"
open System.IO
open System
#r "nuget: Newtonsoft.Json"
open Newtonsoft.Json

type jsonObj = {
    defaultFunctionName:string
    description:string
    name:string
    language:string
    category:string List
    categoryStyle:string
    enabledInTryMode:bool
    userPrompt:string List
}

type jsonObjList = jsonObj List

let fileContent = 
    use sr = new StreamReader("metadata2.json")
    sr.ReadToEnd()

let myobj = JsonConvert.DeserializeObject<jsonObjList>(fileContent)    
let x= myobj.Head
let newList = x::myobj



let myjson = JsonConvert.SerializeObject(newList, Formatting.Indented)

let writeFile =
        use writer = new StreamWriter("metadata2.json")
        writer.Write myjson
                        







let readLines = seq{
        use sr = new StreamReader("extract_CES_SAE_v3_p0606183nh9k_20220901111527.txt")
        while not sr.EndOfStream do
            yield sr.ReadLine () 
    }


let firstLine = readLines|> Seq.tryHead
let lineCount (fl:string) = 
    let flsplit = fl.Split '|' 
    match flsplit|>Array.length with
    | 5 -> match Int32.TryParse flsplit[2] with
            | false, _ -> None
            | true, v -> Some v
    | _ -> None

let oup="open.txt"

let r =
    match firstLine |> Option.bind lineCount with
    | Some 0 -> ()
    | Some v when v > 0  ->
        //let blockBlob = outputContainer.GetBlobClient("formatted-out/"+name)    
        //use str = blockBlob.OpenWrite(true,new Models.BlobOpenWriteOptions())
                        use writer = new StreamWriter(oup, false)
                        readLines
                        |> Seq.iter (fun line -> writer.WriteLine line)

    | _ -> ()    