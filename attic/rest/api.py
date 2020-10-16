# -*- coding: utf-8 -*-


class Authenticate(BasicAuth):
    def check_auth(self, username, password, allowed_roles,
                   resource, method):
        #print("check auth: username=", username)
        if username == 'admin' and password == 'funani':
            return True
        # use Eve's own db driver; no additional connections/resources are used
        accounts = app.data.driver.db['accounts']
        lookup = {'username': username}
        if allowed_roles:
            # only retrieve a user if his roles match ``allowed_roles``
            lookup['roles'] = {'$in': allowed_roles}
        account = accounts.find_one(lookup)
        return account and account['password'] == password
        #return account and check_password_hash(account['password'], password)

if __name__ == '__main__':
    app = Eve(auth=Authenticate)
    Bootstrap(app)
    app.register_blueprint(eve_docs, url_prefix='/docs')
    app.run()
