
// Required for importing json files into typescript file.
// Used by token-order-payment
// specifically token-payment-appsettings.json &
// token-payment-options.json

declare module '*.json' {
  const value: any;
  export default value;
}
