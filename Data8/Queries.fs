namespace Data8

    module Queries=

        open System.IO

        open FSharp
        open FSharp.Data.Runtime
        open FSharp.Data

        open Data8.Types

        let getCountriesParameter(countryList: string list ) =
            let res = countryList |> List.exists((=) "all")
            match  res with
            | true -> "all"
            | false -> String.concat ";" countryList

        let getYearsParameter(startYear: string, endYear: string ) =   
            match startYear, endYear with
            | "all", _ -> ""
            | _, "all" -> ""
            | _, _ -> sprintf "&date=%s:%s" startYear endYear


        let getPageParameter(page: int ) =
            match page with
            | 0 ->  ""
            | _ -> sprintf "&page=%i" page

        let getUrl(countriesList: string list, indicator: string, startYear: string, endYear: string, page: int) =
            let url = "http://api.worldbank.org/countries/" + getCountriesParameter(countriesList) +
                        "/indicators/" + indicator + "?format=xml" + getYearsParameter(startYear, endYear) + getPageParameter(page) 
            url

        let getUrlList(countriesList: string list, indicator: string, startYear: string, endYear: string, endPage: int) =
            let urls = [1..endPage] |> List.map(fun x -> getUrl(countriesList,indicator,startYear,endYear,x))
            urls

        let downLoad(url: string)=
            let indicatorPage1 = theIndicatorProvider.Load(url)
            indicatorPage1    

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


        let loadIndicatorList(url: string)= 
            let someIndicators = downLoad(url)
            [ for anIndicator in someIndicators.Datas do
                        yield cleanIndicator(anIndicator) ]


        let loadIndicator(countriesList: string list, indicator: string, startYear: string, endYear: string)=
            let url = getUrl(countriesList,indicator,startYear,endYear,0)
            let indicatorPage1 = downLoad(url)
            let urlList = getUrlList(countriesList,indicator,startYear,endYear,indicatorPage1.Pages)
            let fullList = urlList |> List.map (fun u -> loadIndicatorList(u))
            let flatList = fullList |> List.concat
            flatList

        let loadCountryNamesFromCache()=
            let countryNames = theCountriesCsvProvider.Load(@".\..\..\Cache\Countries.tsv")
            countryNames.Rows |> Seq.toList


        let loadIndicatorNamesFromCache()=
            let indicatorNames = theIndicatorNamesCsvProvider.Load(@".\..\..\Cache\IndicatorNames.tsv")
            indicatorNames.Rows |> Seq.toList

