namespace Composite.Test

open System.IO
open System.Configuration

open Newtonsoft.Json.Linq

module DataSourceConfigurationManager =

    module Github =
        
        let config_path = ConfigurationManager.AppSettings.["GithubConfigPath"]

        type Config () =
            let config = JObject.Parse(File.ReadAllText(config_path))

            member this.Username = config.SelectToken("$.user") |> string
            member this.Repository = config.SelectToken("$.repository") |> string
