# CargaEmbarques

![Platform](https://img.shields.io/badge/platform-Android-green)
![Framework](https://img.shields.io/badge/framework-Xamarin.Android-blue)
![Language](https://img.shields.io/badge/language-C%23-purple)
![Architecture](https://img.shields.io/badge/type-Mobile%20Enterprise%20App-orange)
![Status](https://img.shields.io/badge/status-Active%20Development-brightgreen)

## Overview

**CargaEmbarques** is a mobile application developed with
**Xamarin.Android** designed to support operational processes related to
**shipment loading and logistics verification**.

The application allows field operators and supervisors to capture
shipment data, verify trailers, record photographic evidence, and
calculate axle weight distribution directly from Android devices used in
logistics operations.

## Main Features

- Shipment and order capture from mobile devices
- Trailer verification and operational control
- Axle weight distribution calculation for transport units
- Photo capture and upload for operational evidence
- Integration with enterprise SOAP Web Services
- Microsoft Teams notifications via webhooks
- Local data persistence for field operations

## Technology Stack

- **Framework:** Xamarin.Android
- **Language:** C#
- **Target Android Version:** Android 13
- **Serialization:** Newtonsoft.Json
- **HTTP Communication:** System.Net.Http
- **Device Information:** Xam.Plugin.DeviceInfo
- **Media Capture:** Xam.Plugin.Media
- **UI Components:** AndroidX AppCompat, CardView

## Architecture

The application follows a modular structure based on:

- Activities and Fragments for UI navigation
- Adapters for data presentation
- Service integrations for enterprise communication
- Local storage components for offline capability

## External Integrations

The application communicates with internal enterprise services through
SOAP endpoints for:

- Shipment management
- Trailer verification
- Photo evidence storage

It also includes integration with **Microsoft Teams** to send
operational notifications.

## Project Structure (Simplified)

    MainActivity.cs
    CapturarPedido.cs
    Frmtrailer.cs
    PesoXEjesFragment.cs
    PesoXEjesFragment2.cs
    PesoAdapter.cs
    GuardarLocal.cs
    TeamsNotifier.cs
    WebhookServer.cs
    Web References/
    Resources/
    Assets/

## Build Requirements

- Visual Studio with **Xamarin.Android**
- Android SDK compatible with **API Level 33**
- .NET Framework compatible with Xamarin.Android

## Status

This project is currently used in **operational logistics environments**
and continues to evolve with improvements focused on reliability and
operational efficiency.

## License

Private repository -- internal enterprise use only.
