$testpeople = @(
  [pscustomobject]@{
    FirstName = 'Maisy '
    LastName  = 'Lyon'
    Username  = 'mlyon'
    Age = 24
    Allowed = 'true'
    DateOfBirth = (Get-Date -Year 1999 -Month 7 -Day 14)
  }

  [pscustomobject]@{
    FirstName = 'Stefanie'
    LastName  = 'Mercer'
    Username  = 'smercer'
    Age = 54
    Allowed = $true
    DateOfBirth = '1953-01-01T00:00:00Z'
  }

  [pscustomobject]@{
    FirstName = 'Wilbur'
    LastName  = 'Hanson'
    Username  = 'whanson'
    Age = '36'
    Allowed = $false
    DateOfBirth = '12/12/1956'
  }
)

$testpeople | Select-Object -Property FirstName, LastName, Username, Age, Allowed, DateOfBirth