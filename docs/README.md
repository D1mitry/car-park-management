# Car park management API

## Run the Solution
To run the solution locally, clone or pull it from the repository, open it in Visual Studio, and run it from there. The Swagger page will automatically open in your browser

## Setup
In the appsettings.json file, the configuration for the total available parking spaces can be found under the section:
```json
"Parking": {
    "TotalSpaces": <value>
}
```
You can set TotalSpaces to any positive integer in the range of `ushort` c# type.

The pricing configuration is under the `Charging` section in appsettings.json, which includes two subsections:
- `Basic` – the basic charge per minute, depending on the vehicle type.
- `Additional` – additional charges defined under the `Charge` section, applied every specified `Interval` minutes.
```json
"Charging": {
    "Basic": {
        "SmallCar": <value>,
        "MediumCar": <value>,
        "LargeCar": <value>
    },
    "Additional": {
        "Interval": <minutes>,
        "Charge": <value>
    }
}
```

## Assumptions
- The parking managed by this API has **only one entrance and one exit**.
- Each parking space is large enough to accommodate a **Large Car**.
- As stated, *"Vehicles will be charged per minute they are parked,"* charges are applied **once a full minute has passed**.
- Using an In-Memory Database should be sufficient.

## Question
For the charge calculation:
- Should it be applied starting from the first minute or after each full minute?
- The same clarification applies to the additional charge: should it be applied exactly after each interval has passed, or counted from the start minute?