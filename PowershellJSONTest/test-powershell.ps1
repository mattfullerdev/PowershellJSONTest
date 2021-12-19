$testpeople = @(
  [pscustomobject]@{
    FirstName = 'Maisy '
    LastName  = 'Lyon'
    Username  = 'mlyon'
  }

  [pscustomobject]@{
    FirstName = 'Stefanie'
    LastName  = 'Mercer'
    Username  = 'smercer'
  }

  [pscustomobject]@{
    FirstName = 'Wilbur'
    LastName  = 'Hanson'
    Username  = 'whanson'
  }
)

$testpeople | Export-Csv -NoTypeInformation -Path "test-csv.csv"