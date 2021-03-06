var boardGameApp = new Vue({
    el: '#boardGameApp',
    data: {
        playerRole: sessionStorage.getItem("sessionPlayerRole"),
        isAuthorized: false,
        isAdmin: false,
        loading: true,
        boardgames: [],
        allCategories: [],
        allArtists: [],
        currentBoardGame: null,
        selectedCategories: [],
        selectedArtists: [],
        apiMessageInfo: "",
        searchBoardGameTitle: "",
        isDisabled: true,
        hasError: false,
        hasSuccess: false,
        editBoardGame: false,
        boardgamesCount: 0,

    },
    created:
        function () {
            let self = this;
            self.FetchBoardGames();
            self.IsPlayerAuthorizedBoardGames();
        },
    methods: {
        FetchBoardGames:
            function () {
                let self = this;
                axios
                    .get(`${boardGameApiURL}`)
                    .then(function (response) {
                        self.boardgames = response.data;
                        self.loading = true;
                        self.boardgamesCount = response.data.length;
                    })
                    .catch(function (error) {
                        console.log(error.response.data.title);
                    })
                    .finally(function () {
                        setTimeout(function () {
                            self.loading = false;
                        }, 1000);
                    });
            },
        FetchBoardGameByTitle:
            function () {
                let self = this;
                let getBoardGameByNameApiUrl = `${boardGameApiURL}?title=${self.searchBoardGameTitle}`;
                axios.get(`${getBoardGameByNameApiUrl}`)
                    .then(function (response) {
                        self.boardgames = response.data;
                        self.boardgamesCount = self.boardgames.length;
                    })
                    .catch(function (error) {
                        console.log(error.response);
                        self.searchBoardGameTitle = "";
                        self.FetchBoardGameByTitle();
                    });
            },
        FetchCategories:
            function () {
                let self = this;
                axios.get(`${categoriesApiURL}`, axiosBoardGameConfig)
                    .then(function (response) {
                        self.allCategories = response.data;
                    })
                    .catch(function (error) {
                        console.log(error.response.data.title);
                    });
            },
        FetchArtists:
            function () {
                let self = this;
                axios.get(`${artistsApiURL}`, axiosBoardGameConfig)
                    .then(function (response) {
                        self.allArtists = response.data;
                    })
                    .catch(function (error) {
                        console.log(error.response.data.title);
                    });
            },
        EditBoardGame:
            function (editBoardGame) {
                let self = this;
                self.editBoardGame = editBoardGame;
                self.isDisabled = false;
                if (!self.editBoardGame) {
                    self.currentBoardGame = {
                        title: "",
                        stock: false,
                        playTime: "",
                        photoUrl: "",
                        description: "",
                        isDeleted: false,
                    };
                }
            },
        SaveBoardGame:
            function () {
                let self = this;
                let stock = self.currentBoardGame.stock;
                self.currentBoardGame.stock = (stock === "true") ? true : false;
                if (!self.editBoardGame) {
                    self.PostBoardGame();
                }
                else {
                    self.PutBoardGame();
                }
                    self.selectedCategories.forEach(function (id) {
                        setTimeout(function () { self.PostBoarGameCategories(id); }, 2000);
                    });
                    self.selectedArtists.forEach(function (id) {
                        setTimeout(function () { self.PostBoardGameArtists(id); }, 2000);
                    });
            },
        PostBoardGame:
            function() {
                let self = this;
                let postBoardGameUrl = `${boardGameApiURL}`;
                axios.post(postBoardGameUrl, self.currentBoardGame, axiosBoardGameConfig)
                    .then(function (response) {
                        self.isDisabled = true;
                        self.hasSuccess = true;
                        self.currentBoardGame = response.data;
                        self.apiMessageInfo = `Boardgame with id '${response.data.id}' has been created. Refresh page.!!`
                    })
                    .catch(function (error) {
                        console.log(error.response.data.title);
                        self.hasError = true;
                        self.apiMessageInfo = `${error.response.data.title} Please check values.`;
                    })
                    .finally(function () {
                        setTimeout(function () {
                            self.hasSuccess = false;
                            self.hasError = false;
                        }, 2000)
                    });
            },
        PutBoardGame:
            function () {
                let self = this;
                let putBoardGameUrl = `${boardGameApiURL}`;
                axios.put(putBoardGameUrl, self.currentBoardGame, axiosBoardGameConfig)
                    .then(function (response) {
                        self.isDisabled = true;
                        self.hasSuccess = true;
                        self.apiMessageInfo = `Boardgame with id '${response.data.id}' has been updated. Refresh page.!!`
                    })
                    .catch(function (error) {
                        console.log(error);
                        self.hasError = true;
                        self.apiMessageInfo = `${error.response.data.title} Please check values.`;
                    })
                    .finally(function () {
                        setTimeout(function () {
                            self.hasSuccess = false;
                            self.hasError = false;
                        }, 2000)
                    });
            },
        PostBoarGameCategories:
            function(categoryId) {
                let self = this;
                let postBoardGameCategoryUrl = `${boardGameApiURL}/${self.currentBoardGame.id}/Categories/${categoryId}`;
                axios.post(postBoardGameCategoryUrl, [self.currentBoardGame.id, categoryId], axiosBoardGameConfig)
                    .then(function (response) {
                        console.log("Category was added to BoardGame");
                    })
                    .catch(function (error) {
                        console.log(error.response);
                    });
            },
        PostBoardGameArtists:
            function (artistId) {
                let self = this;
                let postBoardGameArtistUrl = `${boardGameApiURL}/${self.currentBoardGame.id}/Artists/${artistId}`;
                axios.post(postBoardGameArtistUrl, [self.currentBoardGame.id, artistId], axiosBoardGameConfig)
                    .then(function (response) {
                        console.log("Artist was added to Boardgame.");
                    })
                    .catch(function (error) {
                        console.log(error.response.data.title);
                    });
            },
        DeleteBoardGame:
            function () {
                let self = this;
                let deleteBoardGameUrl = `${boardGameApiURL}/${self.currentBoardGame.id}`;
                axios.delete(deleteBoardGameUrl, axiosBoardGameConfig)
                    .then(function (response) {
                        self.boardgames.forEach(function (boardGame, index) {
                            if (boardGame.id === self.currentBoardGame.id) {
                                self.boardgames.splice(index, 1);
                            }
                        });
                        self.boardgamesCount = self.boardgames.length;
                        self.currentBoardGame = null;
                    })
                    .catch(function (error) {
                        console.log(error);
                        self.apiMessageInfo = error;
                        self.hasError = true;
                    })
                    .finally(function () {
                        setTimeout(function () {
                            self.hasSuccess = false;
                            self.hasError = false;
                        }, 2500);
                    });
            },
        IsPlayerAuthorizedBoardGames:
            function () {
                let self = this;
                switch (self.playerRole) {
                    case "Admin":
                        self.isAuthorized = true;
                        self.isAdmin = true;
                        break;
                    case "BoardGameEditor":
                        self.isAuthorized = true;
                        break;
                    case "ArtistEditor":
                        self.isAuthorized = false;
                        break;
                    case "Player":
                        self.isAuthorized = false;
                        break;
                    default:
                }
            },
        IsInStock:
            function (boardGame) {
                let self = this;
                if (boardGame.stock == "True") {
                    return "badge badge-success";
                }
                else {
                    return "text-hide";
                }
            },
        GetBoardGameDetails:
            function (boardGame) {
                let self = this;
                self.isDisabled = true;
                self.currentBoardGame = boardGame;
                self.FetchCategories();
                self.FetchArtists();
            }
    }
});
