$(function () {
    $('#input_apiKey').off();
    $('#input_apiKey').on('change', function () {
        var key = this.value;
        if (key && key.trim() !== '') {
            key = 'Bearer ' + key;
            window.authorizations.add('key', new SwaggerClient.ApiKeyAuthorization('Authorization', key, 'header'));
        }
    });
})();