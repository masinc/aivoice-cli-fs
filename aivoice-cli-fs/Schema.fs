module Schema

open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Unicode

let DEFAULT_VOLUME = 1.0
let DEFAULT_PITCH = 1.0
let DEFAULT_SPEED = 1.0
let DEFAULT_PITCH_RANGE = 1.0
let DEFAULT_MIDDLE_PAUSE = 150.0
let DEFAULT_LONG_PAUSE = 370.0
let DEFAULT_SENTENCE_PAUSE = 800.0

type MasterControl() =
    member val Volume: double = DEFAULT_VOLUME with get, set
    member val Pitch: double = DEFAULT_PITCH with get, set
    member val Speed: double = DEFAULT_SPEED with get, set
    member val PitchRange: double = DEFAULT_PITCH_RANGE with get, set
    member val MiddlePause: double = DEFAULT_MIDDLE_PAUSE with get, set
    member val LongPause: double = DEFAULT_LONG_PAUSE with get, set
    member val SentencePause: double = DEFAULT_SENTENCE_PAUSE with get, set

    member this.to_json() : string =
        let opt = JsonSerializerOptions()
        opt.Encoder <- JavaScriptEncoder.Create UnicodeRanges.All
        opt.WriteIndented <- true
        JsonSerializer.Serialize(this, opt)

    static member from_json(json: string) : MasterControl = JsonSerializer.Deserialize json


type Info() =
    member val name: string = null with get, set
    member val volume: double = DEFAULT_VOLUME with get, set
    member val pitch: double = DEFAULT_PITCH with get, set
    member val speed: double = DEFAULT_SPEED with get, set
    member val pitch_range: double = DEFAULT_PITCH_RANGE with get, set
    member val middle_pause: double = DEFAULT_MIDDLE_PAUSE with get, set
    member val long_pause: double = DEFAULT_LONG_PAUSE with get, set
    member val sentence_range: double = DEFAULT_SENTENCE_PAUSE with get, set

    member this.to_json() : string =
        let opt = JsonSerializerOptions()
        opt.Encoder <- JavaScriptEncoder.Create UnicodeRanges.All
        opt.WriteIndented <- true
        JsonSerializer.Serialize(this, opt)

    member this.to_master_control() : MasterControl =
        MasterControl(
            Volume = this.volume,
            Pitch = this.pitch,
            Speed = this.speed,
            PitchRange = this.pitch_range,
            MiddlePause = this.middle_pause,
            LongPause = this.long_pause,
            SentencePause = this.sentence_range
        )

    static member from_json(json: string) : Info = JsonSerializer.Deserialize json
