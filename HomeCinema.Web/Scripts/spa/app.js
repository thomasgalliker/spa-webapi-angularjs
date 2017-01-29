(function () {
    'use strict';

    angular.module('homeCinema', ['common.core', 'common.ui'])
        .config(config)
        .run(run);

    config.$inject = ['$routeProvider', '$provide'];
    function config($routeProvider, $provide) {

        $provide.decorator('$exceptionHandler', ['$log', '$delegate',
              function ($log, $delegate) {
                  return function (exception, cause) {
                      //$log.debug(exception);
                      //TODO: Log unhandled exception here
                      $delegate(exception, cause);
                  };
              }
        ]);


        $routeProvider
            .when("/", {
                templateUrl: "scripts/spa/home/index.html",
                controller: "indexCtrl"
            })
            .when("/#", {
            })
            .when('/error/400',
            {
                templateUrl: 'scripts/spa/errors/error400.html'
            })
            .when('/error/403',
            {
                templateUrl: 'scripts/spa/errors/error403.html'
            })
            .when('/error/404',
            {
                templateUrl: 'scripts/spa/errors/error404.html'
            })
            .when('/error/500',
            {
                templateUrl: 'scripts/spa/errors/error500.html'
            })
            .when("/about", {
                templateUrl: "scripts/spa/home/about.html",
                controller: "aboutCtrl"
            })
            .when("/login", {
                templateUrl: "scripts/spa/account/login.html",
                controller: "loginCtrl"
            })
            .when("/account/register", {
                templateUrl: "scripts/spa/account/register.html",
                controller: "registerCtrl"
            })
            .when("/account/edit/:id/", {
                templateUrl: "scripts/spa/account/accountDetails.html",
                controller: "accountDetailsCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })
            .when("/account/add", {
                templateUrl: "scripts/spa/account/accountDetails.html",
                controller: "accountDetailsCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })
            .when("/account/users", {
                templateUrl: "scripts/spa/account/users.html",
                controller: "usersCtrl"
            })
            .when("/customers", {
                templateUrl: "scripts/spa/customers/customers.html",
                controller: "customersCtrl"
            })
            .when("/customers/register", {
                templateUrl: "scripts/spa/customers/register.html",
                controller: "customersRegCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })
            .when("/movies", {
                templateUrl: "scripts/spa/movies/movies.html",
                controller: "moviesCtrl"
            })
            .when("/movies/add", {
                templateUrl: "scripts/spa/movies/add.html",
                controller: "movieAddCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })
            .when("/movies/:id", {
                templateUrl: "scripts/spa/movies/details.html",
                controller: "movieDetailsCtrl",
                resolve: { isAuthenticated: isAuthenticated }
            })
            .when("/movies/edit/:id", {
                templateUrl: "scripts/spa/movies/edit.html",
                controller: "movieEditCtrl"
            })
            .when("/rental", {
                templateUrl: "scripts/spa/rental/rental.html",
                controller: "rentStatsCtrl"
            }).otherwise({ redirectTo: "/" });
    }

    run.$inject = ['$rootScope', '$location', '$cookieStore', '$http'];

    function run($rootScope, $location, $cookieStore, $http) {
        // handle page refreshes
        $rootScope.repository = $cookieStore.get('repository') || {};
        if ($rootScope.repository.loggedUser) {
            $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;
        }

        $(document).ready(function () {
            $(".fancybox").fancybox({
                openEffect: 'none',
                closeEffect: 'none'
            });

            $('.fancybox-media').fancybox({
                openEffect: 'none',
                closeEffect: 'none',
                helpers: {
                    media: {}
                }
            });

            $('[data-toggle=offcanvas]').click(function () {
                $('.row-offcanvas').toggleClass('active');
            });
        });

        // see what's going on when the route tries to change
        $rootScope.$on('$routeChangeStart', function (event, next, current) {
            // next is an object that is the route that we are starting to go to
            // current is an object that is the route where we are currently

            //var currentPath = current.originalPath;
            //var nextPath = next.originalPath;

            //console.log('Starting to leave %s to go to %s', currentPath, nextPath);
        });

        $rootScope.$on('$routeChangeError', function (evt, current, previous, rejection) {
            console.log('Route error', rejection);
        });

        $rootScope.$on('routeChangeSuccess', function (evt, current, previous) {
            console.log('Route success');
        });
    }

    isAuthenticated.$inject = ['membershipService', '$rootScope', '$location'];

    function isAuthenticated(membershipService, $rootScope, $location) {
        if (!membershipService.isUserLoggedIn()) {
            $rootScope.previousState = $location.path();
            $location.path('/login');
        }
    }

})();