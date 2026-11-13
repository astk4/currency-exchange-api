export default {
  async fetch(request, env, ctx) {
    
    const url = new URL(request.url);
    const targetUrl = env.API_ADDRESS + url.pathname + url.search;

    // clones the original request and sends it to actual api address
    const modifiedRequest = new Request(targetUrl, {
      method: request.method,
      headers: request.headers,
      body: request.body,
      redirect: 'follow'
    });

    return fetch(modifiedRequest);
  },
};