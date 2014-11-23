namespace Data8

    module DownLoader=
        open System.IO
        open FSharp
        open FSharp.Data.Runtime
        open FSharp.Data

        open Data8.Types

        let countriesFromPage(number: int)= 
            let name = sprintf @".\..\..\Test Data World Bank\countries %02d.xml" number
            let someCountries = theCountriesProvider.Load(name)
            [ for aCountry in someCountries.Countries do
                                            yield new Country( aCountry.Region.Value, aCountry.Name, aCountry.Iso2Code ) ]


        let cacheCountries()=
            let countries = theCountriesProvider.Load(@".\..\..\Test Data World Bank\countries 01.xml")
            let countryData = [1..countries.Pages] |> List.map (fun n -> countriesFromPage( n ) )
            let countryDataFlat = countryData |> List.concat
            let countryStrings = countryDataFlat |> List.map (fun x -> x.tsvString())
            let header = "Country\tRegion\tCode"
            let linesToWrite = List.append [header] countryStrings
            File.WriteAllLines(@".\..\..\Cache\Countries.tsv", linesToWrite)
            linesToWrite



        let topicsFromPage(number: int)= 
            let name = sprintf @".\..\..\Test Data World Bank\topics %02d.xml" number
            let someTopics = theTopicsProvider.Load(name)
            [ for aTopic in someTopics.Topics do
                yield new Topic( aTopic.Value ) ]

        let cacheTopics()=
            let topics = theTopicsProvider.Load(@".\..\..\Test Data World Bank\topics 01.xml")
            let topicData = [1..topics.Pages] |> List.map (fun n -> topicsFromPage( n ) )
            let topicDataFlat = topicData |> List.concat
            let topicStrings = topicDataFlat |> List.map (fun x -> x.tsvString())
            let header = "Topic"
            let linesToWrite = List.append [header] topicStrings
            File.WriteAllLines(".\..\..\Cache\Topics.tsv", linesToWrite)
            linesToWrite

        let firstTopic(anIndicator: theIndicatorDefinitionsProvider.Indicator) =
            let name = anIndicator.Name
            let id = anIndicator.Id
            let t = [for t2 in anIndicator.Topics do yield t2.Value]
            let topicName = 
                if t.Length > 0 
                then t.Head.ToString()
                else "blank"
            new IndicatorDefinition(name, topicName, id )


        let indicatorDefinitionsFromPage(number: int)= 
            let name = sprintf @".\..\..\Test Data World Bank\indicators %02d.xml" number
            let someIndicatorDefinitions = theIndicatorDefinitionsProvider.Load(name)
            [ for anIndicatorDefintion in someIndicatorDefinitions.Indicators do
                yield firstTopic(anIndicatorDefintion) ]

        let cacheIndicatorDefinitions()=
            let indicatorDefinitions = theIndicatorDefinitionsProvider.Load(@".\..\..\Test Data World Bank\indicators 01.xml")    
            let indicatorDefinitionsData = [1..indicatorDefinitions.Pages] |> List.map (fun n -> indicatorDefinitionsFromPage( n ) )
            let indicatorDefinitionsDataFlat = indicatorDefinitionsData |> List.concat
            let indicatorDefinitionsDataStrings = indicatorDefinitionsDataFlat |> List.map (fun x -> x.tsvString())
            let header = "Topic\tIndicator\tID"
            let linesToWrite = List.append [header] indicatorDefinitionsDataStrings
            File.WriteAllLines(".\..\..\Cache\IndicatorNames.tsv", linesToWrite)
            linesToWrite

        let cleanIndicator(anIndicator: theIndicatorProvider.Data2)=
            let id = anIndicator.Indicator.Id
            let indicatorName = anIndicator.Indicator.Value
            let countryId = anIndicator.Country.Id
            let country = anIndicator.Country.Value
            let aDate = anIndicator.Date
            let indicatorValue =
                try
                    anIndicator.Value
                with
                    | :? System.Exception -> 0.0m
            new Indicator( id, indicatorName, countryId, country, aDate, indicatorValue)


        let indicatorFromPage(number: int)= 
            let name = @".\..\..\Test Data World Bank\1.0.HCount.1.25usd.xml"
            let someIndicators = theIndicatorProvider.Load(name)
            [ for anIndicator in someIndicators.Datas do
                    yield cleanIndicator(anIndicator) ]


        let cacheIndicator()=
            let indicator = theIndicatorProvider.Load(@".\..\..\Test Data World Bank\1.0.HCount.1.25usd.xml")
            let indicatorData = [1..indicator.Pages] |> List.map (fun n -> indicatorFromPage( n ) )
            let indicatorDataFlat = indicatorData |> List.concat
            let indicatorStrings = indicatorDataFlat |> List.map (fun x -> x.tsvString())
            let header = "Topic\tIndicator"
            let indicatorToWrite = List.append [header] indicatorStrings
            File.WriteAllLines(@".\..\..\Test Out\1.0.HCount.1.25usd.tsv", indicatorToWrite)
            indicatorToWrite







