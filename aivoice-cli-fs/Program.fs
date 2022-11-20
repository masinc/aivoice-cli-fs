open System.Collections.Generic
open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Unicode
open AI.Talk.Editor.Api
open CommandLine
open CommandLine.Text

[<RequireQualifiedAccess>]
type OutputFormat =
    | text = 0
    | json = 1

[<Verb("list-char")>]
type CliListChar =
    { [<Option('o', "output", Default = OutputFormat.text)>]
      output: OutputFormat }
    member this.run(tts: TtsControl) =
        let names = tts.VoiceNames

        match this.output with
        | OutputFormat.text ->
            names
            |> Array.iter (fun name -> printfn $"{name}")
        | OutputFormat.json ->
            let opt = JsonSerializerOptions()
            opt.Encoder <- JavaScriptEncoder.Create UnicodeRanges.All
            opt.WriteIndented <- true
            let s = JsonSerializer.Serialize(names, opt)
            printfn $"{s}"
        | _ -> failwith "unreachable"

        0

[<Verb("list-preset")>]
type CliListPreset =
    { [<Option('o', "output", Default = OutputFormat.text)>]
      output: OutputFormat }
    member this.run(tts: TtsControl) =
        let names = tts.VoicePresetNames

        match this.output with
        | OutputFormat.text ->
            names
            |> Array.iter (fun name -> printfn $"{name}")
        | OutputFormat.json ->
            let opt = JsonSerializerOptions()
            opt.Encoder <- JavaScriptEncoder.Create UnicodeRanges.All
            opt.WriteIndented <- true
            let s = JsonSerializer.Serialize(names, opt)
            printfn $"{s}"
        | _ -> failwith "unreachable"

        0

[<Verb("info")>]
type CliInfo =
    { [<Option('v', "verbose", Default = false)>]
      verbose: bool }
    member this.run(tts: TtsControl) =
        let j =
            Schema.MasterControl.from_json tts.MasterControl

        printfn $"{j.to_json ()}"
        0

[<Verb("play")>]
type CliPlay() =
    [<Option('n', "name")>]
    member val name: string = null with get, set

    [<Option('t', "text", Group = "input")>]
    member val text: string = null with get, set

    [<Option('f', "file", Group = "input")>]
    member val file: string = null with get, set

    member this.run(tts: TtsControl) =
        let text =
            if this.text <> null then
                this.text
            else
                System.IO.File.ReadAllText this.file

        if this.name <> null then
            tts.CurrentVoicePresetName <- this.name

        tts.TextEditMode <- TextEditMode.Text
        tts.Text <- text
        tts.Play()
        0

[<Verb("save")>]
type CliSave() =
    [<Option('n', "name")>]
    member val name: string = null with get, set

    [<Option('t', "text", Group = "input")>]
    member val text: string = null with get, set

    [<Option('f', "file", Group = "input")>]
    member val file: string = null with get, set

    [<Option('o', "output", Required = true)>]
    member val output: string = null with get, set
    
    member this.run(tts: TtsControl) =
        let text =
            if this.text <> null then
                this.text
            else
                System.IO.File.ReadAllText this.file
        if this.name <> null then
            tts.CurrentVoicePresetName <- this.name
        
        tts.TextEditMode <- TextEditMode.Text
        tts.Text <- text
        
        tts.SaveAudioToFile(this.output)
        0

let init_tts (tts: TtsControl) =
    let hostname =
        tts.GetAvailableHostNames()[0]

    tts.Initialize hostname

    if tts.Status = HostStatus.NotRunning then
        tts.StartHost()

[<EntryPoint>]
let main args =
    let tts = TtsControl()

    try
        init_tts tts
        tts.Connect()

        let result =
            Parser.Default.ParseArguments<CliListChar, CliListPreset, CliInfo, CliPlay, CliSave> args

        match result with
        | :? CommandLine.Parsed<obj> as command ->
            match command.Value with
            | :? CliListChar as opts -> opts.run tts
            | :? CliListPreset as opts -> opts.run tts
            | :? CliInfo as opts -> opts.run tts
            | :? CliPlay as opts -> opts.run tts
            | :? CliSave as opts -> opts.run tts
            | _ -> failwith "unreachable"
        | :? CommandLine.NotParsed<obj> -> 1
        | _ -> failwith "unreachable"
    finally
        tts.Disconnect()
