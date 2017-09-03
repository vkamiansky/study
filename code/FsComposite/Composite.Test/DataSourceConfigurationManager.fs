namespace Composite.Test

open System.IO
open System.Configuration

open Newtonsoft.Json.Linq

module DataSourceConfigurationManager =

    let getJsonValue (config: JObject) key =
        config.SelectToken(sprintf "$.%s" key) |> string
    
    module Github =
        
        let config_path = ConfigurationManager.AppSettings.["GithubConfigPath"]

        type Config () =
            let config = JObject.Parse(File.ReadAllText(config_path))
            let getJsonValueFromConfig = getJsonValue config

            member this.Username = getJsonValueFromConfig "user"
            member this.Password = getJsonValueFromConfig "password"
            member this.Repository = getJsonValueFromConfig "repository"
