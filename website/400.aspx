<%--
    Catch dodgy requests which would cause the 500.aspx page to error
    e.g. http://localhost:53291/biotopes<script></script>
    Redirect to the 500 page
--%>
<%@ Page validateRequest="false" %>
<% Response.StatusCode = 400; %>
<!-- #include file="500.html" -->