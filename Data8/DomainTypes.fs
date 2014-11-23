namespace Data8

    module Types=

        open FSharp
        open FSharp.Data.Runtime
        open FSharp.Data

        type theCountriesProvider = XmlProvider<".\Test Data World Bank\countries 01.xml">
        type theIndicatorDefinitionsProvider = XmlProvider<".\Test Data World Bank\indicators 01.xml">
        type theTopicsProvider = XmlProvider<".\Test Data World Bank\1topics.xml">
        type theIndicatorProvider = XmlProvider<".\Test Data World Bank\Brazil gdp.NY.GDP.MKTP.CD.xml">
        type theCountriesCsvProvider = CsvProvider<".\Cache\Countries.tsv">
        type theIndicatorNamesCsvProvider = CsvProvider<".\Cache\IndicatorNames.tsv", "\t">

        type Country (Name: string, Region: string, Code: string) =    
            member this.tsvString()=
                let s = sprintf "%s\t%s\t%s" Region Name Code
                s

        type Topic (Name: string) =
            member this.tsvString()=
                Name

        type Indicator (id: string, name: string, country_code: string, country: string, year: int, value: decimal ) = class
            member this.id = id
            member this.name = name
            member this.country_code = country_code
            member this.country = country
            member this.year = year
            member this.value = value

            member this.tsvString()=
                let s = sprintf "%s\t%s\t%s\t%s\t%d\t%f" id name country_code country year value
                s

            member this.Id
                with get() = id
    
            member this.Name
                with get() = name
    
            member this.Country
                with get() = country_code

            member this.Year
                with get() = year

            member this.Value
                with get() = value
        end

        type IndicatorDefinition = class
            val Name: string
            val TopicName: string
            val Id:string
            new (name: string, topicName: string, id: string) =
                { Name = name.Trim();
                TopicName = topicName.Trim();
                Id = id }   
            member this.tsvString()=
                let s = sprintf "%s\t%s\t%s" this.TopicName this.Name this.Id
                s
        end


