using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace UniteHere.eMemberRegApp {
  public class PreflightRequestsHandler : DelegatingHandler {
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
      if (request.Headers.Contains("Origin") && request.Method.Method == "OPTIONS") {
        var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept, Authorization");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS, PATCH");
        var tsc = new TaskCompletionSource<HttpResponseMessage>();
        tsc.SetResult(response);
        return tsc.Task;
      }
      return base.SendAsync(request, cancellationToken);
    }
  }
}