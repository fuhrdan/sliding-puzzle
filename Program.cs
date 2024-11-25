//*****************************************************************************
//** 773. Sliding Puzzle          leetcode                                   **
//*****************************************************************************


// Queue node structure
typedef struct Node {
    char state[7];
    int pos;
    int steps;
    struct Node* next;
} Node;

// Queue structure
typedef struct Queue {
    Node* front;
    Node* rear;
} Queue;

// Function prototypes
Queue* createQueue();
void enqueue(Queue* queue, const char* state, int pos, int steps);
Node* dequeue(Queue* queue);
bool isEmpty(Queue* queue);
void freeQueue(Queue* queue);

typedef struct HashSet {
    char** keys;
    int capacity;
    int size;
} HashSet;

HashSet* createHashSet(int capacity);
bool hashSetContains(HashSet* set, const char* key);
void hashSetInsert(HashSet* set, const char* key);
void freeHashSet(HashSet* set);

int BFS(char* initState, int pos, HashSet* visited);
void serializeBoard(int** board, int boardSize, int boardColSize, char* state, int* pos);

// BFS search
int BFS(char* initState, int pos, HashSet* visited) {
    const char* target = "123450";
    if (strcmp(initState, target) == 0) return 0;

    // Moves for each position
int moves[6][4] = {
    {1, 3, -1, -1},  // Position 0
    {0, 2, 4, -1},   // Position 1
    {1, 5, -1, -1},  // Position 2
    {0, 4, -1, -1},  // Position 3
    {1, 3, 5, -1},   // Position 4
    {2, 4, -1, -1}   // Position 5
};

    Queue* queue = createQueue();
    enqueue(queue, initState, pos, 0);
    hashSetInsert(visited, initState);

    while (!isEmpty(queue)) {
        Node* current = dequeue(queue);
        char curState[7];
        strcpy(curState, current->state);
        int curPos = current->pos;
        int curSteps = current->steps;
        free(current);

        for (int i = 0; moves[curPos][i] != -1; i++) {
            int nextPos = moves[curPos][i];

            // Swap positions
            char newState[7];
            strcpy(newState, curState);
            char temp = newState[curPos];
            newState[curPos] = newState[nextPos];
            newState[nextPos] = temp;

            if (strcmp(newState, target) == 0) {
                freeQueue(queue);
                return curSteps + 1;
            }

            if (!hashSetContains(visited, newState)) {
                hashSetInsert(visited, newState);
                enqueue(queue, newState, nextPos, curSteps + 1);
            }
        }
    }

    freeQueue(queue);
    return -1;
}

// Serialize the board into a string
void serializeBoard(int** board, int boardSize, int boardColSize, char* state, int* pos) {
    int idx = 0;
    for (int i = 0; i < boardSize; i++) {
        for (int j = 0; j < boardColSize; j++) {
            state[idx] = board[i][j] + '0';
            if (board[i][j] == 0) {
                *pos = idx;
            }
            ++idx;
        }
    }
    state[6] = '\0'; // Null-terminate the string
}

// Sliding puzzle main function
int slidingPuzzle(int** board, int boardSize, int* boardColSize) {
    char initState[7];
    int pos;
    serializeBoard(board, boardSize, *boardColSize, initState, &pos);

    HashSet* visited = createHashSet(1024);
    int result = BFS(initState, pos, visited);

    freeHashSet(visited);
    return result;
}

// Queue implementation
Queue* createQueue() {
    Queue* queue = (Queue*)malloc(sizeof(Queue));
    queue->front = queue->rear = NULL;
    return queue;
}

void enqueue(Queue* queue, const char* state, int pos, int steps) {
    Node* newNode = (Node*)malloc(sizeof(Node));
    strcpy(newNode->state, state);
    newNode->pos = pos;
    newNode->steps = steps;
    newNode->next = NULL;

    if (queue->rear == NULL) {
        queue->front = queue->rear = newNode;
        return;
    }

    queue->rear->next = newNode;
    queue->rear = newNode;
}

Node* dequeue(Queue* queue) {
    if (queue->front == NULL) return NULL;

    Node* temp = queue->front;
    queue->front = queue->front->next;

    if (queue->front == NULL) {
        queue->rear = NULL;
    }

    return temp;
}

bool isEmpty(Queue* queue) {
    return queue->front == NULL;
}

void freeQueue(Queue* queue) {
    while (!isEmpty(queue)) {
        Node* temp = dequeue(queue);
        free(temp);
    }
    free(queue);
}

// HashSet implementation
HashSet* createHashSet(int capacity) {
    HashSet* set = (HashSet*)malloc(sizeof(HashSet));
    set->capacity = capacity;
    set->size = 0;
    set->keys = (char**)calloc(capacity, sizeof(char*));
    return set;
}

bool hashSetContains(HashSet* set, const char* key) {
    for (int i = 0; i < set->capacity; ++i) {
        if (set->keys[i] && strcmp(set->keys[i], key) == 0) {
            return true;
        }
    }
    return false;
}

void hashSetInsert(HashSet* set, const char* key) {
    for (int i = 0; i < set->capacity; ++i) {
        if (!set->keys[i]) {
            set->keys[i] = strdup(key);
            set->size++;
            return;
        }
    }
}

void freeHashSet(HashSet* set) {
    for (int i = 0; i < set->capacity; i++) {
        if (set->keys[i]) free(set->keys[i]);
    }
    free(set->keys);
    free(set);
}