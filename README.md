# Xumm External Authentication plugin for nopCommerce

## Easy Configuration
1. [Sign up](https://apps.xumm.dev/) for a Xumm Developer account, if you don't already have one.
2. Install the plugin in nopCommerce.
3. Configure the API Credentials and make sure the Redirect URL is set.
4. You're all set to use Xumm for external authentication.

# Instructions

## Step 1: Register a Xumm Developer account

[Sign up](https://apps.xumm.dev/) for a Xumm Developer account, if you don't already have one.

Once you have access to the Xumm Developer Dashboard, you will be able to create an application with the required API Credentials.

The credentials can be found inside the Xumm Developer Dashboard > Settings > Application credentials section.

After that, please update your Redirect URI's with `https://<your-store-url>/signin-xumm` in the Application details section.

## Step 2: Install the plugin in nopCommerce
The Xumm Pay plugin is not (yet) available in the nopCommerce Marketplace so you need to create a folder `ExternalAuth.Xumm` in the nopCommerce Plugins folder and copy the contents of `src`.

More details can be found in the [nopCommerce Documentation](https://docs.nopcommerce.com/en/getting-started/advanced-configuration/plugins-in-nopcommerce.html).

## Step 3: Configuration Xumm plugin
After installing the plugin, you can navigate to Configuration > Authentication > External authentication on the navigation menu of the admin panel.

You will be able to see “Xumm authentication” on the list, click Configure.

### API Settings

#### API Credentials
First you need to configure the API Key and Secret found of the Application credentials section at the [Developer Dashboard](https://apps.xumm.dev/). 

#### Redirect URL
Update the Redirect URI's list at the Application details section of the [Developer Dashboard](https://apps.xumm.dev/) with the Redirect URL shown at the plugin configuration page.

## Step 4: Validate Xumm authentication Configuration
The [API Key](#api-credentials), [API Secret](#api-credentials) and [Webhook URL](#redirect-url) has to be configured as required. 

The Xumm authentication method will not be visible at the login page for consumers if not configured correctly.
