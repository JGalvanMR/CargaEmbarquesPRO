# 🚛 CargaEmbarques

![Platform](https://img.shields.io/badge/platform-Android-green)
![Framework](https://img.shields.io/badge/framework-Xamarin.Android-blue)
![Language](https://img.shields.io/badge/language-C%23-purple)
![Architecture](https://img.shields.io/badge/type-Enterprise%20Mobile%20App-orange)
![Status](https://img.shields.io/badge/status-Active%20Development-brightgreen)

------------------------------------------------------------------------

## 📖 Overview

CargaEmbarques is a mobile enterprise application used to support
shipment loading operations and logistics verification from Android
devices.

------------------------------------------------------------------------

## 🚚 Core Functionalities

  Feature                    Description
  -------------------------- ----------------------------
  Shipment Capture           Register shipment data
  Trailer Verification       Validate transport units
  Axle Weight Distribution   Calculate axle loads
  Photo Evidence             Capture operational images
  Teams Notifications        Send alerts via webhooks

------------------------------------------------------------------------

## 🏗 System Architecture

``` mermaid
flowchart LR
Operator --> MobileApp
MobileApp --> LocalStorage
MobileApp --> EnterpriseServices
EnterpriseServices --> LogisticsDatabase
EnterpriseServices --> MicrosoftTeams
```

------------------------------------------------------------------------

## ⚙️ Technology Stack

  Component       Technology
  --------------- -----------------
  Platform        Xamarin.Android
  Language        C#
  Serialization   Newtonsoft.Json
  Communication   System.Net.Http
  UI              AndroidX

------------------------------------------------------------------------

## Project Structure

    MainActivity.cs
    CapturarPedido.cs
    Frmtrailer.cs
    PesoXEjesFragment.cs
    PesoAdapter.cs
    Resources/
    Assets/

------------------------------------------------------------------------

## License

Private repository -- internal enterprise use.
