// http://stackoverflow.com/questions/881515/how-do-i-declare-a-namespace-in-javascript
function namespace(ns) {
    var object = this, tokens = ns.split("."), token;

    while (tokens.length > 0) {
        token = tokens.shift();

        if (typeof object[token] === "undefined") {
            object[token] = {};
        }

        object = object[token];
    }

    return object;
}